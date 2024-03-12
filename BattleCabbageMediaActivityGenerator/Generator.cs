using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BattleCabbageMediaActivityGenerator.Models;
using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;

namespace BattleCabbageMediaActivityGenerator
{
    internal sealed class Generator : IGenerator
    {
        private readonly Models.BattleCabbageVideoContext _context;
        private readonly ILogger _logger;

        private Randomizer _r = new Randomizer();

        // Generation Settings
        private int _dailyUserGenerationCount = 17;
        private int _rentalsToday = 500 * 9;
        private int _rentalsThisLoop = 500 * 9 / 288;
        private float _returnChance = 0.8f;
        private int _returnsToday;
        private int _returnsThisLoop;

        // Cancel rate is 8%, we divide by 28 to get daily cancel rate
        private decimal _cancelRate = (0.08m/30);

        // New subscription rate is 8.2%, we dive by 28 to get daily new subscription rate
        private decimal _newSubRate = (0.082m/30);

        private bool _renewals = false;

        private bool _weekend = false;
        private DateTime _LastGenerationDate = DateTime.Now.Date;

        private List<Models.Movie> newReleases = new List<Models.Movie>();
        private List<Models.Movie> recentMovies = new List<Models.Movie>();
        private List<Models.Movie> olderMovies = new List<Models.Movie>();

        private List<Models.User> users = new List<Models.User>();

        public Generator(Models.BattleCabbageVideoContext context, ILogger<Generator> logger)
        {
            _context = context;
            _logger = logger;
            SetGenerationSettings(DateTime.Now.Date, true);
        }

        public Generator(Models.BattleCabbageVideoContext context, ILogger<Generator> logger, DateTime runDate)
        {
            _logger = logger;
            _context = context;
            SetGenerationSettings(runDate, true);
        }

        public async Task GeneratePastActivity(DateTime startDate, DateTime endDate)
        {
            DateTime currentGenerationTime = startDate;
            _logger.LogInformation($"Generating activity from {startDate} to {endDate}");
            while (currentGenerationTime <= endDate)
            {
                _logger.LogInformation($"Generating activity for {currentGenerationTime}. Start Time: {DateTime.Now}");
                await GenerateAsync(currentGenerationTime);
                _logger.LogInformation($"Generated activity for {currentGenerationTime}. End Time: {DateTime.Now}");
                currentGenerationTime = currentGenerationTime.AddMinutes(5);
            }
        }

        public async Task GenerateCurrentActivity()
        {
            while(true)
            {
                await GenerateAsync();
                // Sleep for 5 minutes
                await Task.Delay(5 * 1000 * 60);
            }
            
        }

        public async Task GenerateAsync(DateTime? runDate = null)
        {
            DateTime currentGenerationTime = runDate ?? DateTime.Now;
            SetGenerationSettings(currentGenerationTime);

            var inventoryItems = _context.Inventories.ToList();

            // Locally pull the late fee and rental fee so we don't have to keep hitting the database
            var lateFee = inventoryItems.Where(i => i.InventoryId == 2).First().CurrentPrice;
            var rentalFee = inventoryItems.Where(i => i.InventoryId == 1).First().CurrentPrice;
            var subscriptionFee = inventoryItems.Where(i => i.InventoryId == 3).First().CurrentPrice;

            // 5 minute loop means 288 generations per day
            if (CheckChances(_dailyUserGenerationCount, 288))
            {
                _logger.LogInformation("Creating new user based on randomization.");
                var user_generated = Generators.User.Generate();

                var userSubscriptionStatus = new Models.UserSubscriptionStatus
                {
                    UserId = user_generated.UserId,
                    Active = false,
                    RenewalDay = 0,
                    MostRecentSubscriptionPurchase = null
                };

                if (_r.Float() <= 0.8)
                {
                    _logger.LogInformation("Newly created user has activated subscription.");
                    //80% chance of a new user subscribing to streaming
                    var purchase = new Models.Purchase
                    {
                        PurchasingUserId = user_generated.UserId,
                        TransactionCreatedOn = currentGenerationTime,
                        PurchaseLocationId = 0,
                        PaymentCardId = user_generated.UserCreditCards.First().CreditCardId
                    };

                    var purchaseLineItem = new Models.PurchaseLineItem
                    {
                        ItemId = 3,
                        Quantity = 1,
                        TotalPrice = subscriptionFee
                    };

                    userSubscriptionStatus.Active = true;
                    userSubscriptionStatus.MostRecentSubscriptionPurchase = purchaseLineItem;
                    userSubscriptionStatus.RenewalDay = currentGenerationTime.Day > 28 ? 1 : currentGenerationTime.Day;

                    purchase.PurchaseLineItems.Add(purchaseLineItem);
                    await _context.Purchases.AddAsync(purchase);
                    await _context.PurchaseLineItems.AddAsync(purchaseLineItem);
                    
                }
                await _context.UserSubscriptionStatuses.AddAsync(userSubscriptionStatus);
                await _context.Users.AddAsync(user_generated);
                await _context.SaveChangesAsync();
            }

            if (_r.Decimal() <= _newSubRate)
            { 
                _logger.LogInformation("Creating new subscription based on randomization.");
                var _potentialNewSubscriber = _context.UserSubscriptionStatuses.Where(u => u.Active == false).Include(u => u.User).ThenInclude(uc => uc.UserCreditCards).OrderBy(u => Guid.NewGuid()).First();
                _potentialNewSubscriber.Active = true;
                _potentialNewSubscriber.RenewalDay = currentGenerationTime.Day > 28 ? 1 : currentGenerationTime.Day;

                var purchase = new Models.Purchase
                {
                    PurchasingUserId = _potentialNewSubscriber.UserId,
                    TransactionCreatedOn = currentGenerationTime,
                    PurchaseLocationId = 0,
                    PaymentCardId = _potentialNewSubscriber.User.UserCreditCards.First().CreditCardId
                };

                var purchaseLineItem = new Models.PurchaseLineItem
                {
                    ItemId = 3,
                    Quantity = 1,
                    TotalPrice = subscriptionFee
                };

                _potentialNewSubscriber.MostRecentSubscriptionPurchase = purchaseLineItem;

                purchase.PurchaseLineItems.Add(purchaseLineItem);
                await _context.Purchases.AddAsync(purchase);
                await _context.PurchaseLineItems.AddAsync(purchaseLineItem);
                _context.Update(_potentialNewSubscriber);
                await _context.SaveChangesAsync();
            }


            await CreateRentals(lateFee, rentalFee, currentGenerationTime);

            await CreateReturns(lateFee, currentGenerationTime);

            if (_renewals)
                await CreateRenewals(subscriptionFee, currentGenerationTime);
        }

        public async Task CreateRentals(decimal lateFee, decimal rentalFee, DateTime? runDate = null)
        {
            DateTime currentGenerationTime = runDate ?? DateTime.Now;
            _logger.LogInformation($"Generating rentals for {currentGenerationTime}");
            int totalRentals = _rentalsThisLoop;
            int totalUsers = 0;
            int returnCount = 0;
            // Generate rentals for users based on the number of rentals we should have per loop
            // To have a rental, we need to make a purchase, so we'll generate a purchase for each rental
            // To do that we also need to decide on how many rentals a user is making (1 - 3)
            // All this to say, we will per loop pick a user, pick a number of rentals, and generate that many rentals
            // Those will then be added to the Purchase
            for (int i = 0; i < _rentalsThisLoop; i++)
            {
                _logger.LogDebug($"Generating rental {i + 1} of {_rentalsThisLoop}");
                totalUsers++;
                _logger.LogDebug($"Choosing the user");
                var user = users.OrderBy(u => Guid.NewGuid()).First();
                _logger.LogDebug($"User chosen: {user.UserId}");
                _logger.LogDebug($"Checking for unreturned rentals");
                var unReturnedRentals = user.Rentals.Where(r => r.Return == null);
                _logger.LogDebug($"Unreturned rentals found: {unReturnedRentals.Count()}");
                _logger.LogDebug($"Choosing the kiosk");
                var kiosk = _context.Kiosks.Where(k => k.State == user.UserAddresses.First().State).OrderBy(k => Guid.NewGuid()).First();
                _logger.LogDebug($"Kiosk chosen: {kiosk.KioskId}");
                int rentalCount = _r.Number(1, 3);
                var purchase = new Models.Purchase
                {
                    PurchasingUserId = user.UserId,
                    TransactionCreatedOn = currentGenerationTime,
                    PurchaseLocationId = kiosk.KioskId,
                    PaymentCardId = user.UserCreditCards.First().CreditCardId
                };
                var returnLateCount = 0;
                if (unReturnedRentals.Count() != 0)
                {
                    foreach (var rental in unReturnedRentals)
                    {
                        _logger.LogDebug($"Adding return for rental {rental.RentalId}");
                        rental.Return = new Models.Return
                        {
                            ReturnDate = currentGenerationTime,
                            LateDays = (currentGenerationTime - rental.ExpectedReturnDate).Days < 0 ? 0 : (currentGenerationTime - rental.ExpectedReturnDate).Days
                        };
                        if (rental.Return.LateDays > 0)
                        {
                            // Max days charged is 30
                            var lateDaysCharged = (rental.Return.LateDays > 30 ? 30 : rental.Return.LateDays);
                            PurchaseLineItem return_purchase = new Models.PurchaseLineItem
                            {
                                ItemId = 2,
                                Quantity = lateDaysCharged,
                                TotalPrice = lateDaysCharged * lateFee
                            };
                            purchase.PurchaseLineItems.Add(return_purchase);
                            rental.Return.LateChargeLineItem = purchase.PurchaseLineItems.ElementAt(returnLateCount);
                            returnLateCount++;
                        }                        
                        await _context.Returns.AddAsync(rental.Return);
                        _context.Rentals.Update(rental);
                        returnCount++;
                        
                    }
                }
                for (int j = 0; j < rentalCount; j++)
                {
                    _logger.LogDebug($"Adding Purchase {j + 1} of {rentalCount}");
                    PurchaseLineItem purchaselineitem = new Models.PurchaseLineItem
                    {
                        ItemId = 1,
                        Quantity = 1,
                        TotalPrice = rentalFee
                    };
                    purchase.PurchaseLineItems.Add(purchaselineitem);
                }
                await _context.Purchases.AddAsync(purchase);
                for (int j = 0; j < rentalCount; j++)
                {
                    _logger.LogDebug($"Adding Rental {j + 1} of {rentalCount}");
                    var rental = new Models.Rental
                    {
                        UserId = user.UserId,
                        MovieId = GetRandomMovie().MovieId,
                        RentalDate = currentGenerationTime,
                        ExpectedReturnDate = currentGenerationTime.AddDays(3),
                        PurchaseLineItem = purchase.PurchaseLineItems.ElementAt(j + returnLateCount)
                    };
                    await _context.Rentals.AddAsync(rental);
                }
                await _context.SaveChangesAsync();
                i += rentalCount;
            }
            _logger.LogInformation($"Generated {totalRentals} rentals for {totalUsers} users with {returnCount} returns.");
        }

        public async Task CreateReturns(decimal lateFee, DateTime? runDate = null)
        {
            var currentGenerationTime = runDate ?? DateTime.Now;
            _logger.LogInformation($"Generating returns for {currentGenerationTime}");

            var potentialReturns = await _context.Rentals.Where(r => r.Return == null).Include(p => p.PurchaseLineItem).ThenInclude(pl => pl.Purchase).Include(r => r.User).ThenInclude(u => u.UserCreditCards).ToListAsync();

            if (potentialReturns.Count() == 0)
            {
                return;
            }

            for (int i = 0; i < _returnsThisLoop; i++)
            {
                var rental = potentialReturns.OrderBy(r => Guid.NewGuid()).First();
                rental.Return = new Models.Return
                {
                    ReturnDate = currentGenerationTime,
                    LateDays = (currentGenerationTime - rental.ExpectedReturnDate).Days < 0 ? 0 : (currentGenerationTime - rental.ExpectedReturnDate).Days
                };
                if (rental.Return.LateDays > 0)
                {
                    var purchase = new Models.Purchase
                    {
                        PurchasingUserId = rental.UserId,
                        TransactionCreatedOn = currentGenerationTime,
                        PurchaseLocationId = rental.PurchaseLineItem.Purchase.PurchaseLocationId,
                        PaymentCardId = rental.User.UserCreditCards.First().CreditCardId
                    };
                    // Max days charged is 30
                    var lateDaysCharged = (rental.Return.LateDays > 30 ? 30 : rental.Return.LateDays);
                    rental.Return.LateChargeLineItem = new Models.PurchaseLineItem
                    {
                        ItemId = 2,
                        Quantity = lateDaysCharged,
                        TotalPrice = lateDaysCharged * lateFee
                    };
                    purchase.PurchaseLineItems.Add(rental.Return.LateChargeLineItem);
                }
                await _context.Returns.AddAsync(rental.Return);
                _context.Rentals.Update(rental);


                await _context.SaveChangesAsync();
            }
            _logger.LogInformation($"Generated {_returnsThisLoop} returns.");
        }

        public async Task CreateRenewals(decimal subscriptionFee, DateTime? runDate = null)
        {
            var currentGenerationTime = runDate ?? DateTime.Now;
            _logger.LogInformation($"Generating subscription renewals for {currentGenerationTime}");
            var potentialRenewals = await _context.UserSubscriptionStatuses.Where(u => u.Active == true && u.RenewalDay == currentGenerationTime.Day).Include(u => u.User).ThenInclude(u => u.UserCreditCards).ToListAsync();

            int actualRenewals = 0;
            int cancellations = 0;
            if (potentialRenewals.Count() == 0)
            {
                return;
            }

            foreach (var user in potentialRenewals)
            {
                if (_r.Decimal() > _cancelRate)
                {
                    var purchase = new Models.Purchase
                    {
                        PurchasingUserId = user.UserId,
                        TransactionCreatedOn = currentGenerationTime,
                        PurchaseLocationId = 0,
                        PaymentCardId = user.User.UserCreditCards.First().CreditCardId
                    };

                    var purchaseLineItem = new Models.PurchaseLineItem
                    {
                        ItemId = 3,
                        Quantity = 1,
                        TotalPrice = subscriptionFee
                    };

                    purchase.PurchaseLineItems.Add(purchaseLineItem);

                    user.MostRecentSubscriptionPurchase = purchaseLineItem;

                    await _context.Purchases.AddAsync(purchase);
                    await _context.PurchaseLineItems.AddAsync(purchaseLineItem);
                    actualRenewals++;
                } else
                {
                    // Cancel their subscription
                    user.Active = false;
                    user.RenewalDay = 0;
                    _context.Update(user);
                    cancellations++;
                }
            }
            _renewals = false;
            _logger.LogInformation($"Generated {actualRenewals} renewals and {cancellations} cancellations.");
            await _context.SaveChangesAsync();
        }
        public void SetGenerationSettings(DateTime? runDate = null, bool initialization = false)
        {
            DateTime currentGenerationTime = runDate ?? DateTime.Now.Date;

            if (currentGenerationTime.Date > _LastGenerationDate || initialization)
            {
                _LastGenerationDate = currentGenerationTime.Date;
                _renewals = true;
                _weekend = currentGenerationTime.DayOfWeek == DayOfWeek.Thursday || currentGenerationTime.DayOfWeek == DayOfWeek.Friday || currentGenerationTime.DayOfWeek == DayOfWeek.Saturday;
                _dailyUserGenerationCount = _r.Number(11, 34);

                if (_weekend)
                {
                    _rentalsToday = _context.Kiosks.Count() * _r.Number(30, 65);
                }
                else
                {
                    _rentalsToday = _context.Kiosks.Count() * _r.Number(4, 12);
                }

                newReleases = _context.Movies.Where(m => m.ReleaseDate > DateOnly.FromDateTime(currentGenerationTime.AddDays(-14))).ToList();
                recentMovies = _context.Movies.Where(m => m.ReleaseDate > DateOnly.FromDateTime(currentGenerationTime.AddDays(-90)) && m.ReleaseDate <= DateOnly.FromDateTime(currentGenerationTime.AddDays(-14))).ToList();
                olderMovies = _context.Movies.Where(m => m.ReleaseDate <= DateOnly.FromDateTime(currentGenerationTime.AddDays(-90))).ToList();

                users = _context.Users.Include(a => a.UserAddresses.Where(ua => ua.Default == true)).Include(c => c.UserCreditCards.Where(uc => uc.Default == true)).Include(r => r.Rentals).AsSplitQuery().ToList();

                _returnsToday = (int)(_context.Rentals.Where(r => r.Return == null).Count() * _returnChance);
            }
            int rentalModifier;
            if (_weekend)
            {
                rentalModifier = _r.Number(-17, 17);
            }
            else
            {
                rentalModifier = _r.Number(-3, 3);
            }
            _rentalsThisLoop = (_rentalsToday / 288) + rentalModifier;
            _returnsThisLoop = (_returnsToday / 288);
        }

        public bool CheckChances(int instance_rate, int total_rate)
        {
            float percent = (float)instance_rate / (float)total_rate;
            float chance = _r.Float();
            return percent >= chance;
        }

        // This function will be used to generate a random movie based on the provided date and popularity weights
        public Models.Movie GetRandomMovie()
        {
            // 50% chance of getting a new release, 30% chance of getting a recent movie, 20% chance of getting an older movie
            // Once in there, determine the movie based on popularity weights
            float age_chance = _r.Float();
            float pop_chance = _r.Float();
            if (age_chance <= 0.5)
            {
                if (pop_chance <= 0.3)
                {
                    return newReleases.Where(m => m.PopularityScore <= 7).OrderBy(m => Guid.NewGuid()).First();
                }
                else
                {
                    return newReleases.Where(m => m.PopularityScore > 7).OrderBy(m => Guid.NewGuid()).First();
                }
            }
            else if (age_chance <= 0.8)
            {
                if (pop_chance <= 0.3)
                {
                    return recentMovies.Where(m => m.PopularityScore <= 7).OrderBy(m => Guid.NewGuid()).First();
                }
                else
                {
                    return recentMovies.Where(m => m.PopularityScore > 7).OrderBy(m => Guid.NewGuid()).First();
                }
            }
            if (pop_chance <= 0.3)
            {
                return olderMovies.Where(m => m.PopularityScore <= 7).OrderBy(m => Guid.NewGuid()).First();
            }
            else
            {
                return olderMovies.Where(m => m.PopularityScore > 7).OrderBy(m => Guid.NewGuid()).First();
            }
        }
    }
}

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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;

namespace BattleCabbageMediaActivityGenerator
{
    internal sealed class Generator : IHostedService, IHostedLifecycleService
    {
        private readonly IDbContextFactory<BattleCabbageVideoContext> _contextFactory;
        private readonly Models.BattleCabbageVideoContext _context;
        private readonly ILogger _logger;
        private readonly IConfiguration configuration;
        IHostApplicationLifetime _lifeTime;

        private CancellationTokenSource tokenSource = new CancellationTokenSource();

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
        private List<Models.User> users_that_can_rent = new List<Models.User>();

        private List<Models.Rental> potentialReturns = new List<Models.Rental>();

        private Kiosk internet_kiosk = new Kiosk();

        public Generator(IDbContextFactory<BattleCabbageVideoContext> contextFactory, ILogger<Generator> logger, IConfiguration config, IHostApplicationLifetime lifeTime)
        {
            _contextFactory = contextFactory;
            configuration = config;
            _context = _contextFactory.CreateDbContext();
            _logger = logger;
            _lifeTime = lifeTime;
            
            lifeTime.ApplicationStopping.Register(() =>
            {
                _logger.LogInformation("Stopping generation via ApplicationStopping.");
                tokenSource.Cancel();
            });

            if (_context.Kiosks.Find(Guid.Parse("A4A10FC3-B6A1-44F6-ACD4-3E73F0224BDC")) != null)
            {
#nullable disable
                internet_kiosk = _context.Kiosks.Find(Guid.Parse("A4A10FC3-B6A1-44F6-ACD4-3E73F0224BDC"));
#nullable enable
                
            } else
            {
                _logger.LogError("No internet kiosk found.");
                throw new Exception("No internet kiosk found.");
            }
        }

        async Task IHostedService.StartAsync(CancellationToken cancellationToken)
        {
            if (configuration != null && configuration.GetValue<bool>("GEN_PAST_DATA"))
            {
                DateTime startDate = configuration.GetValue<DateTime>("GEN_PAST_START_DATETIME");
                if (startDate == DateTime.MinValue)
                {
                    _logger.LogError("No start date found.");
                    return;
                }
                DateTime endDate = configuration.GetValue<DateTime>("GEN_PAST_END_DATETIME");
                if (endDate == DateTime.MinValue)
                {
                    endDate = DateTime.Now;
                }

                bool resumeGeneration = configuration.GetValue<bool>("GEN_RESUME_GENERATION");
                if (resumeGeneration)
                {
                    // Get the last transaction date from the database to resume generation from that point.
                    if(_context.Purchases.Where(p => p.TransactionCreatedOn >= startDate && p.TransactionCreatedOn <= endDate).Count() > 0)
                        startDate = _context.Purchases.Where(p => p.TransactionCreatedOn >= startDate && p.TransactionCreatedOn <= endDate).OrderByDescending(p => p.TransactionCreatedOn).First().TransactionCreatedOn;
                }
                try
                {
                    await GeneratePastActivity(startDate, endDate, tokenSource.Token);
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine($"\n{nameof(OperationCanceledException)} thrown\n");
                }
                finally
                {
                    tokenSource.Dispose();
                }

            }
            else if (configuration != null)
            {
                try
                {
                    await GenerateCurrentActivity(tokenSource.Token);
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine($"\n{nameof(OperationCanceledException)} thrown\n");
                }
                finally
                {
                    tokenSource.Dispose();
                }

            }
            else
            {
                Console.WriteLine("No configuration found.");
                return;
            }
        }

        public async Task GeneratePastActivity(DateTime startDate, DateTime endDate, CancellationToken token)
        {
            _logger.LogInformation($"Building initial generation settings.");
            DateTime currentGenerationTime = startDate;
            SetGenerationSettings(currentGenerationTime, true);
            _logger.LogInformation($"Generating activity from {startDate} to {endDate}");
            while (currentGenerationTime < endDate)
            {
                _logger.LogInformation($"Generating activity for {currentGenerationTime}. Start Time: {DateTime.Now}");
                await GenerateAsync(currentGenerationTime);
                _logger.LogInformation($"Generated activity for {currentGenerationTime}. End Time: {DateTime.Now}");
                currentGenerationTime = currentGenerationTime.AddMinutes(5);
                if (token.IsCancellationRequested)
                {
                    _logger.LogWarning("Cancellation requested. Stopping generation.");
                    token.ThrowIfCancellationRequested();
                }
            }
            _lifeTime.StopApplication();
        }

        public async Task GenerateCurrentActivity(CancellationToken token)
        {
            SetGenerationSettings(DateTime.Now, true);
            while (true)
            {
                await GenerateAsync();
                // Sleep for 5 minutes
                await Task.Delay(5 * 1000 * 60, token);
                if (token.IsCancellationRequested)
                {
                    _logger.LogWarning("Cancellation requested. Stopping generation.");
                    token.ThrowIfCancellationRequested();
                }
            }
            
        }

        public async Task GenerateAsync(DateTime? runDate = null)
        {


            DateTime currentGenerationTime = runDate ?? DateTime.Now;
            SetGenerationSettings(currentGenerationTime);

            var inventoryItems = _context.Inventories.ToList();

            // Locally pull the late fee and rental fee so we don't have to keep hitting the database
            var lateFee = inventoryItems.Where(i => i.Id == Guid.Parse("40A9154F-E4F5-489A-A123-186742C27E91")).First().CurrentPrice;
            var rentalFee = inventoryItems.Where(i => i.Id == Guid.Parse("7D2E030E-9F97-4ED0-AB21-5C46D93E840E")).First().CurrentPrice;
            var subscriptionFee = inventoryItems.Where(i => i.Id == Guid.Parse("62384FE9-3A3D-4580-9199-41F2ECD5E318")).First().CurrentPrice;

            // 5 minute loop means 288 generations per day
            if (CheckChances(_dailyUserGenerationCount, 288))
            {
                _logger.LogInformation("Creating new user based on randomization.");
                var user_generated = Generators.User.Generate();
                user_generated.CreatedOn = currentGenerationTime;
                user_generated.MemberSince = currentGenerationTime;
                user_generated.ModifiedOn = currentGenerationTime;

                var userSubscriptionStatus = new Models.UserSubscriptionStatus
                {
                    User = user_generated,
                    Active = false,
                    RenewalDay = 0,
                    MostRecentSubscriptionPurchase = null
                };

                if (_r.Float() <= 0.8)
                {
                    _logger.LogInformation("Newly created user has activated subscription.");
                    //80% chance of a new user subscribing to streaming
                    // Pick them up in tomorrows renewal charge run
                    userSubscriptionStatus.Active = true;
                    userSubscriptionStatus.RenewalDay = currentGenerationTime.Day + 1 > 28 ? 1 : currentGenerationTime.Day + 1;
                    
                }
                user_generated.UserSubscriptionStatus = userSubscriptionStatus;
                await _context.AddAsync(user_generated);
            }

            if (_r.Decimal() <= _newSubRate)
            { 
                _logger.LogInformation("Creating new subscription based on randomization.");
                var _potentialNewSubscriber = _context.UserSubscriptionStatuses.Where(u => u.Active == false).Include(u => u.User).ThenInclude(uc => uc.UserCreditCards).OrderBy(u => Guid.NewGuid()).First();
                _potentialNewSubscriber.Active = true;
                _potentialNewSubscriber.RenewalDay = currentGenerationTime.Day + 1 > 28 ? 1 : currentGenerationTime.Day + 1;

                _context.Entry(_potentialNewSubscriber).State = EntityState.Modified;
            }


            await CreateRentals(lateFee, rentalFee, currentGenerationTime);

            await CreateReturns(lateFee, currentGenerationTime);

            if (_renewals)
                await CreateRenewals(subscriptionFee, currentGenerationTime);

            _logger.LogInformation("Saving all changes to database.");
            var saved = false;
            while (!saved)
            {
                try
                {
                    // Attempt to save changes to the database
                    await _context.SaveChangesAsync();
                    saved = true;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    foreach (var entry in ex.Entries)
                    {
                        _logger.LogWarning("Conflict detected for {0}", entry.Entity);
                            var proposedValues = entry.CurrentValues;
                            var databaseValues = entry.GetDatabaseValues();
                            
                            foreach (var property in proposedValues.Properties)
                            {
                                var proposedValue = proposedValues[property];
                                
                                if (databaseValues != null)
                                {
                                    var databaseValue = databaseValues[property];
                                }
                                
                                _logger.LogWarning("Conflict detected for {0}", property);
                                // TODO: decide which value should be written to database
                                proposedValues[property] = proposedValue;
                            }

                            // Refresh original values to bypass next concurrency check
                            entry.OriginalValues.SetValues(proposedValues);
                    }
                }
            }

        }

        public async Task CreateRentals(decimal lateFee, decimal rentalFee, DateTime? runDate = null)
        {
            DateTime currentGenerationTime = runDate ?? DateTime.Now;
            _logger.LogInformation($"Generating rentals for {currentGenerationTime}");
            int totalRentals = _rentalsThisLoop;
            int totalUsers = 0;
            int returnCount = 0;

            var rentalList = new List<Models.Rental>();
            var kioskList = new List<Guid>();
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

                var user = _r.ListItem(users_that_can_rent);


                users_that_can_rent.Remove(user);
                
                _logger.LogDebug($"User chosen: {user.Id}");

                _logger.LogDebug($"Checking for unreturned rentals");
                // To handle this running multiple copies at once for faster loading, we are ignoring the unreturned rentals older than two months
                var unReturnedRentals = user.Rentals.Where(r => r.Return == null && r.RentalDate > currentGenerationTime.AddDays(-14) && r.RentalDate < currentGenerationTime).ToList();
                _logger.LogDebug($"Unreturned rentals found: {unReturnedRentals.Count()}");

                _logger.LogDebug($"Choosing the kiosk");
                
                var kiosk = _context.Kiosks.Where(k => k.State == user.UserAddresses.First().State).OrderBy(k => Guid.NewGuid()).First();


                kioskList.Add(kiosk.Id);

                _logger.LogDebug($"Kiosk chosen: {kiosk.Id}");

                int rentalCount = _r.Number(1, 3);
                var purchase = new Models.Purchase
                {
                    PurchaseLocation = kiosk,
                    TransactionCreatedOn = currentGenerationTime
                };

                purchase.PaymentCards.Add(user.UserCreditCards.First());

                purchase.PurchasingUsers.Add(user);
                _logger.LogDebug($"Creating rentals for user {user.Email}");

                for (int j = 0; j < rentalCount; j++)
                {
                    _logger.LogDebug($"Adding Purchase and Rental {j + 1} of {rentalCount}");
                    PurchaseLineItem purchaselineitem = new Models.PurchaseLineItem
                    {
                        ItemId = Guid.Parse("7D2E030E-9F97-4ED0-AB21-5C46D93E840E"),
                        Quantity = 1,
                        TotalPrice = rentalFee
                    };
                    purchaselineitem.Purchase = purchase;

                    _logger.LogDebug($"Created Purchase Line Item");

                    var movie = GetRandomMovie();

                    _logger.LogDebug($"Movie chosen: {movie.Title}");

                    var rental = new Models.Rental
                    {
                        User = user,
                        Movie = movie,
                        RentalDate = currentGenerationTime,
                        ExpectedReturnDate = currentGenerationTime.AddDays(3)
                    };

                    rental.PurchaseLineItem = purchaselineitem;

                    _logger.LogDebug($"Created Rental Item");

                     rentalList.Add(rental);
                }

                var returnLateCount = 0;
                if (unReturnedRentals.Count() != 0)
                {
                    foreach (var rental in unReturnedRentals)
                    {
                        _logger.LogDebug($"Adding return for rental {rental.Id}");

                        Return return_item = new Models.Return
                        {
                            Rental = rental,
                            ReturnDate = currentGenerationTime,
                            LateDays = (currentGenerationTime - rental.ExpectedReturnDate).Days < 0 ? 0 : (currentGenerationTime - rental.ExpectedReturnDate).Days
                        };
                        if (return_item.LateDays > 0)
                        {
                            // Max days charged is 30
                            var lateDaysCharged = (return_item.LateDays > 30 ? 30 : return_item.LateDays);
                            PurchaseLineItem return_purchase = new Models.PurchaseLineItem
                            {
                                ItemId = Guid.Parse("40A9154F-E4F5-489A-A123-186742C27E91"),
                                Quantity = lateDaysCharged,
                                TotalPrice = lateDaysCharged * lateFee
                            };
                            return_purchase.Purchase = purchase;
                            return_item.LateChargeLineItem = return_purchase;
                            returnLateCount++;
                        }                        
                        await _context.AddAsync(return_item);
                        potentialReturns.Remove(rental);
                        returnCount++;
                    }
                }
                
                i += rentalCount;
            }

            await _context.AddRangeAsync(rentalList);
            _logger.LogInformation($"Generated {totalRentals} rentals for {totalUsers} users with {returnCount} returns.");
        }

        public async Task CreateReturns(decimal lateFee, DateTime? runDate = null)
        {
            var currentGenerationTime = runDate ?? DateTime.Now;
            _logger.LogInformation($"Generating returns for {currentGenerationTime}");

            

            if (potentialReturns.Count() == 0)
            {
                return;
            }
            for (int i = 0; i < _returnsThisLoop; i++)
            {
                var rental = _r.ListItem(potentialReturns);


                _logger.LogDebug($"Adding return for rental {rental.Id}, user {rental.User.Email}");
                Return return_item = new Models.Return
                {
                    Rental = rental,
                    ReturnDate = currentGenerationTime,
                    LateDays = (currentGenerationTime - rental.ExpectedReturnDate).Days < 0 ? 0 : (currentGenerationTime - rental.ExpectedReturnDate).Days
                };
                if (return_item.LateDays > 0)
                {
                    var purchase = new Models.Purchase
                    {
                        TransactionCreatedOn = currentGenerationTime
                    };

                    var kiosk = _context.Kiosks.Where(k => k.Id == rental.PurchaseLineItem.Purchase.PurchaseLocationId).First();

                    purchase.PurchaseLocation = kiosk;


                    purchase.PaymentCards.Add(rental.User.UserCreditCards.First());


                    purchase.PurchasingUsers.Add(rental.User);


                    // Max days charged is 30
                    var lateDaysCharged = (return_item.LateDays > 30 ? 30 : return_item.LateDays);
                    var late_return_line_item = new Models.PurchaseLineItem
                    {
                        ItemId = Guid.Parse("40A9154F-E4F5-489A-A123-186742C27E91"),
                        Quantity = lateDaysCharged,
                        TotalPrice = lateDaysCharged * lateFee
                    };
                    late_return_line_item.Purchase = purchase;
                    return_item.LateChargeLineItem = late_return_line_item;
                    //await _context.AddAsync(purchase);
                }
                await _context.AddAsync(return_item);
                
                potentialReturns.Remove(rental);
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
                        TransactionCreatedOn = currentGenerationTime,
                        PurchaseLocation = internet_kiosk
                    };


                    purchase.PaymentCards.Add(user.User.UserCreditCards.First());


                    purchase.PurchasingUsers.Add(user.User);


                    var purchaseLineItem = new Models.PurchaseLineItem
                    {
                        ItemId = Guid.Parse("62384FE9-3A3D-4580-9199-41F2ECD5E318"),
                        Quantity = 1,
                        TotalPrice = subscriptionFee
                    };

                    purchaseLineItem.Purchase = purchase;

                    user.MostRecentSubscriptionPurchase = purchaseLineItem;


                    await _context.AddAsync(purchase);
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
        }
        public void SetGenerationSettings(DateTime? runDate = null, bool initialization = false)
        {
            DateTime currentGenerationTime = runDate ?? DateTime.Now;

            if (currentGenerationTime.Date > _LastGenerationDate || initialization)
            {
                _LastGenerationDate = currentGenerationTime.Date;
                _context.ChangeTracker.Clear();
                
                // Only process renewals if it's within 3 minutes of midnight
                if(Math.Abs((currentGenerationTime - currentGenerationTime.Date).Minutes) < 3)
                {
                    _renewals = true;
                }

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

                newReleases = _context.Movies.Where(m => m.ReleaseDate > DateOnly.FromDateTime(currentGenerationTime.AddDays(-14)) && m.ReleaseDate <= DateOnly.FromDateTime(currentGenerationTime)).ToList();
                recentMovies = _context.Movies.Where(m => m.ReleaseDate > DateOnly.FromDateTime(currentGenerationTime.AddDays(-90)) && m.ReleaseDate <= DateOnly.FromDateTime(currentGenerationTime.AddDays(-14))).ToList();
                olderMovies = _context.Movies.Where(m => m.ReleaseDate <= DateOnly.FromDateTime(currentGenerationTime.AddDays(-90))).ToList();
                
                // Massive efficiency improvement by removing rentals and returns from Eager loading... Hopefully this doesn't bite me later
                users = _context.Users.Where(u => u.CreatedOn < currentGenerationTime).Include(a => a.UserAddresses.Where(ua => ua.Default == true)).Include(c => c.UserCreditCards.Where(uc => uc.Default == true)).AsSplitQuery().ToList();

                //.Include(a => a.UserAddresses.Where(ua => ua.Default == true)).Include(c => c.UserCreditCards.Where(uc => uc.Default == true)).Include(r => r.Rentals).ThenInclude(rt => rt.Return).AsSplitQuery()

                var rentalCheckDate = currentGenerationTime.AddDays(2);
                users_that_can_rent = users.Where(u => u.Rentals.Count() == 0 || u.Rentals.Where(r => r.ExpectedReturnDate <= rentalCheckDate).Count() > 0 || u.Rentals.Where(r => r.RentalDate >= currentGenerationTime.AddDays(-14)).Count() > 0).ToList();

                potentialReturns = _context.Rentals
                    .Where(r => r.Return == null && r.ExpectedReturnDate <= rentalCheckDate && r.RentalDate > currentGenerationTime.AddDays(-14) && r.RentalDate < currentGenerationTime)
                    .Include(p => p.PurchaseLineItem).ThenInclude(pl => pl.Purchase).Include(r => r.User).ThenInclude(u => u.UserCreditCards).AsSplitQuery().ToList();

                _returnsToday = (int)(potentialReturns.Count() * _returnChance);
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

            if (_context.Kiosks.Find(Guid.Parse("A4A10FC3-B6A1-44F6-ACD4-3E73F0224BDC")) != null)
            {
#nullable disable
                internet_kiosk = _context.Kiosks.Find(Guid.Parse("A4A10FC3-B6A1-44F6-ACD4-3E73F0224BDC"));
#nullable enable

            }
            else
            {
                _logger.LogError("No internet kiosk found.");
                throw new Exception("No internet kiosk found.");
            }

            int returnModifier = (int)(_r.Float(-0.12f, 0.12f) * (_returnsToday/288));

            _rentalsThisLoop = (_rentalsToday / 288) + rentalModifier;
            _returnsThisLoop = (_returnsToday / 288) + returnModifier;
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

        Task IHostedService.StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        Task IHostedLifecycleService.StartingAsync(CancellationToken cancellationToken)
        {
            //_logger.LogInformation("1. StartingAsync has been called.");

            return Task.CompletedTask;
        }

        Task IHostedLifecycleService.StartedAsync(CancellationToken cancellationToken)
        {
            //_logger.LogInformation("3. StartedAsync has been called.");

            return Task.CompletedTask;
        }

        Task IHostedLifecycleService.StoppingAsync(CancellationToken cancellationToken)
        {
            //_logger.LogInformation("6. StoppingAsync has been called.");

            return Task.CompletedTask;
        }

        Task IHostedLifecycleService.StoppedAsync(CancellationToken cancellationToken)
        {
            //_logger.LogInformation("8. StoppedAsync has been called.");

            return Task.CompletedTask;
        }
    }
}

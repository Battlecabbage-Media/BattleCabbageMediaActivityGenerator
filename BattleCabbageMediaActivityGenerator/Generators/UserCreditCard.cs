using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using BattleCabbageMediaActivityGenerator.Models;

namespace BattleCabbageMediaActivityGenerator.Generators
{
    internal static class UserCreditCard
    {
        public static Models.UserCreditCard Generate()
        {
            var newUserCreditCard = new Faker<Models.UserCreditCard>("en")
                .RuleFor(u => u.CreditCardNumber, (f, u) => f.Finance.CreditCardNumber())
                .RuleFor(u => u.CreditCardExpiration, (f, u) => f.Date.Future(4));

            var creditCard = newUserCreditCard.Generate();
            creditCard.CreatedOn = DateTime.Now;
            creditCard.Default = true;

            return creditCard;
        }
    }
}

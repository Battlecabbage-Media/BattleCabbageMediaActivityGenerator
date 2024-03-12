using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BattleCabbageMediaActivityGenerator.Models;
using Bogus;
using Bogus.DataSets;

namespace BattleCabbageMediaActivityGenerator.Generators
{
    internal class User
    {
        public static Models.User Generate(DateTime? memberSince = null)
        {
            DateTime memberAdd = memberSince ?? DateTime.Now.Date;
            var newUser = new Faker<Models.User>("en")
                .RuleFor(u => u.FirstName, (f, u) => f.Name.FirstName(f.Person.Gender))
                .RuleFor(u => u.LastName, (f, u) => f.Name.LastName(f.Person.Gender))
                .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
                .RuleFor(u => u.PhoneNumber, (f, u) => f.Phone.PhoneNumberFormat());
            var user = newUser.Generate();
            user.MemberSince = memberAdd;
            user.UserAddresses = new List<Models.UserAddress>
            {
                UserAddress.Generate()
            };
            user.UserCreditCards = new List<Models.UserCreditCard>
            {
                UserCreditCard.Generate()
            };
            return user;
        }
    }
}

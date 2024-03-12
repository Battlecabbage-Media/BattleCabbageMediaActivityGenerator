using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BattleCabbageMediaActivityGenerator.Models;
using Bogus;

namespace BattleCabbageMediaActivityGenerator.Generators
{
    internal static class UserAddress
    {
        public static Models.UserAddress Generate(DateTime? actionOn = null, bool changeAddress = false)
        {
            DateTime actionTime = actionOn ?? DateTime.Now;
            var newUserAddress = new Faker<Models.UserAddress>()
                .RuleFor(u => u.Address, (f, u) => f.Address.StreetAddress())
                .RuleFor(u => u.State, (f, u) => f.Address.StateAbbr())
                .RuleFor(u => u.ZipCode, (f, u) => f.Address.ZipCode(f.Random.Replace("#####")));

            var userAddress = newUserAddress.Generate();
            userAddress.Default = true;
            if(!changeAddress)
            {
                userAddress.CreatedOn = actionTime;
            }
            userAddress.ModifiedOn = actionTime;
            return userAddress;
        }
    }
}

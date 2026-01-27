using Domain.Common;
using Domain.Common.Domain.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Users.ValueObjects
{
    [Owned]
    public class PhoneNumber : ValueObject
    {
        public string CountryCode { get; }
        public string Number { get; }

        public PhoneNumber(string countryCode, string number)
        {
            CountryCode = countryCode;
            Number = number;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return CountryCode;
            yield return Number;
        }
        private PhoneNumber() { } // For EF
    }
}


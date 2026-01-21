using Domain.Common.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Shipment.ValueObjects
{
    public class Address : ValueObject
    {
        public string Street { get; private set; }        // ✅ ضفت private set
        public string City { get; private set; }          // ✅ ضفت private set
        public string State { get; private set; }         // ✅ ضفت private set
        public string PostalCode { get; private set; }    // ✅ ضفت private set
        public string Country { get; private set; }       // ✅ ضفت private set

        private Address() { } // For EF Core

        public Address(string street, string city, string state, string postalCode, string country)
        {
            Street = street;
            City = city;
            State = state;
            PostalCode = postalCode;
            Country = country;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Street;
            yield return City;
            yield return State;
            yield return PostalCode;
            yield return Country;
        }
    }
}
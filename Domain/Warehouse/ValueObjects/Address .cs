using Domain.Common.Domain.Common;
using Domain.SharedKernel;
using Domain.Warehouse.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Warehouse.ValueObjects
{
    public sealed class Address : ValueObject
    {
        public string Street { get; }
        public string City { get; }
        public string Country { get; }
        public string ZipCode { get; }

        private Address() { }

        private Address(string street, string city, string country, string zipCode)
        {
            Street = street;
            City = city;
            Country = country;
            ZipCode = zipCode;
        }

        public static Result<Address> Create(string street, string city, string country, string zipCode)
        {
            if (string.IsNullOrWhiteSpace(street))
                return Result<Address>.Failure(AddressErrors.EmptyStreet);

            if (string.IsNullOrWhiteSpace(city))
                return Result<Address>.Failure(AddressErrors.EmptyCity);

            if (string.IsNullOrWhiteSpace(country))
                return Result<Address>.Failure(AddressErrors.EmptyCountry);

            if (string.IsNullOrWhiteSpace(zipCode))
                return Result<Address>.Failure(AddressErrors.EmptyZipCode);

            return Result<Address>.Success(new Address(street, city, country, zipCode));
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Street;
            yield return City;
            yield return Country;
            yield return ZipCode;
        }
    }
}

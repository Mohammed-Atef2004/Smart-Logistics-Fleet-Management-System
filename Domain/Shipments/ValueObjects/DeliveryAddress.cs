using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Shipments.ValueObjects
{
    public sealed record DeliveryAddress
    {
        public string Street { get; init; } = default!;
        public string City { get; init; } = default!;
        public string State { get; init; } = default!;
        public string ZipCode { get; init; } = default!;
        public string Country { get; init; } = default!;
        public string? ApartmentUnit { get; init; }

        private DeliveryAddress() { } // EF Core

        private DeliveryAddress(string street, string city, string state, string zipCode, string country, string? apartmentUnit)
        {
            Street = street;
            City = city;
            State = state;
            ZipCode = zipCode;
            Country = country;
            ApartmentUnit = apartmentUnit;
        }

        public static DeliveryAddress Create(
            string street,
            string city,
            string state,
            string zipCode,
            string country,
            string? apartmentUnit = null)
        {
            if (string.IsNullOrWhiteSpace(street)) throw new ArgumentException("Street cannot be empty.", nameof(street));
            if (string.IsNullOrWhiteSpace(city)) throw new ArgumentException("City cannot be empty.", nameof(city));
            if (string.IsNullOrWhiteSpace(state)) throw new ArgumentException("State cannot be empty.", nameof(state));
            if (string.IsNullOrWhiteSpace(zipCode)) throw new ArgumentException("ZipCode cannot be empty.", nameof(zipCode));
            if (string.IsNullOrWhiteSpace(country)) throw new ArgumentException("Country cannot be empty.", nameof(country));

            return new DeliveryAddress(
                street.Trim(), city.Trim(), state.Trim(),
                zipCode.Trim(), country.Trim(), apartmentUnit?.Trim());
        }

        public string FullAddress => ApartmentUnit is not null
            ? $"{Street}, {ApartmentUnit}, {City}, {State} {ZipCode}, {Country}"
            : $"{Street}, {City}, {State} {ZipCode}, {Country}";
    }

}

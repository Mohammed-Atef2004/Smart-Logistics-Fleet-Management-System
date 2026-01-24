using Domain.Common.Domain.Common;

public class Address : ValueObject
{
    public string Street { get; private set; }
    public string City { get; private set; }
    public string State { get; private set; }
    public string Country { get; private set; }
    public string PostalCode { get; private set; }

    private Address() { } // EF Core

    public Address(string street, string city, string state, string country, string postalCode)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new ArgumentException("Street is required", nameof(street));
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City is required", nameof(city));
        if (string.IsNullOrWhiteSpace(country))
            throw new ArgumentException("Country is required", nameof(country));

        Street = street;
        City = city;
        State = state;
        Country = country;
        PostalCode = postalCode;
    }

    public string GetFullAddress() => $"{Street}, {City}, {State} {PostalCode}, {Country}";

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return State ?? string.Empty;
        yield return Country;
        yield return PostalCode ?? string.Empty;
    }
}


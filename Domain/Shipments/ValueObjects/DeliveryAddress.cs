using Domain.SharedKernel;

namespace Domain.Shipments.ValueObjects;

/// <summary>
/// Delivery address value object. Validation returns a <see cref="Result{T}"/>
/// instead of throwing, so an invalid request body becomes a 400 with a
/// domain error code rather than a 500.
/// </summary>
public sealed record DeliveryAddress
{
    public string Street { get; init; } = default!;
    public string City { get; init; } = default!;
    public string State { get; init; } = default!;
    public string ZipCode { get; init; } = default!;
    public string Country { get; init; } = default!;
    public string? ApartmentUnit { get; init; }

    private DeliveryAddress() { } // EF Core

    private DeliveryAddress(
        string street,
        string city,
        string state,
        string zipCode,
        string country,
        string? apartmentUnit)
    {
        Street = street;
        City = city;
        State = state;
        ZipCode = zipCode;
        Country = country;
        ApartmentUnit = apartmentUnit;
    }

    public static Result<DeliveryAddress> Create(
        string street,
        string city,
        string state,
        string zipCode,
        string country,
        string? apartmentUnit = null)
    {
        if (string.IsNullOrWhiteSpace(street))
            return Result<DeliveryAddress>.Failure(Required(nameof(Street)));
        if (string.IsNullOrWhiteSpace(city))
            return Result<DeliveryAddress>.Failure(Required(nameof(City)));
        if (string.IsNullOrWhiteSpace(state))
            return Result<DeliveryAddress>.Failure(Required(nameof(State)));
        if (string.IsNullOrWhiteSpace(zipCode))
            return Result<DeliveryAddress>.Failure(Required(nameof(ZipCode)));
        if (string.IsNullOrWhiteSpace(country))
            return Result<DeliveryAddress>.Failure(Required(nameof(Country)));

        return Result<DeliveryAddress>.Success(new DeliveryAddress(
            street.Trim(),
            city.Trim(),
            state.Trim(),
            zipCode.Trim(),
            country.Trim(),
            string.IsNullOrWhiteSpace(apartmentUnit) ? null : apartmentUnit.Trim()));
    }

    public string FullAddress => ApartmentUnit is not null
        ? $"{Street}, {ApartmentUnit}, {City}, {State} {ZipCode}, {Country}"
        : $"{Street}, {City}, {State} {ZipCode}, {Country}";

    private static Error Required(string field) => new(
        "DeliveryAddress.Required",
        $"{field} is required.");
}

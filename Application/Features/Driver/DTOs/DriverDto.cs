namespace Application.Features.Driver.DTOs;

/// <summary>
/// Read model for a driver. Domain value objects are flattened into
/// primitives here — the API contract must not leak domain types, and
/// AutoMapper cannot map <c>DriverRating</c> to a double implicitly.
/// </summary>
public sealed record DriverDto(
    Guid Id,
    string FullName,
    string Status,
    double Rating,
    int RatingCount,
    string LicenseNumber,
    DateTime LicenseExpiryDate,
    string LicenseCategory,
    Guid? CurrentShiftId);

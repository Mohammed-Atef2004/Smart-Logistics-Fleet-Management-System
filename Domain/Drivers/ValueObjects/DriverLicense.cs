using Domain.Drivers.Errors;
using Domain.SharedKernel;

namespace Domain.Drivers.ValueObjects;

/// <summary>
/// Driver license value object. It is always created through
/// <see cref="Create"/> so an invalid license can never enter the
/// aggregate; the parameterless constructor exists only for EF Core
/// materialization of the owned type.
/// </summary>
public sealed record DriverLicense
{
    /// <summary>Matches the API validator and the database column length.</summary>
    public const int MaxLicenseNumberLength = 20;

    /// <summary>Matches the API validator and the database column length.</summary>
    public const int MaxCategoryLength = 10;

    public string LicenseNumber { get; init; } = default!;
    public DateTime ExpiryDate { get; init; }
    public string Category { get; init; } = default!;

    private DriverLicense() { } // EF Core

    private DriverLicense(string number, DateTime expiry, string category)
    {
        LicenseNumber = number;
        ExpiryDate = expiry;
        Category = category;
    }

    public static Result<DriverLicense> Create(string number, DateTime expiry, string category)
    {
        if (string.IsNullOrWhiteSpace(number))
            return Result<DriverLicense>.Failure(DriverErrors.InvalidLicenseNumber);

        number = number.Trim();
        if (number.Length > MaxLicenseNumberLength)
            return Result<DriverLicense>.Failure(DriverErrors.LicenseNumberTooLong);

        if (string.IsNullOrWhiteSpace(category))
            return Result<DriverLicense>.Failure(DriverErrors.InvalidLicenseCategory);

        category = category.Trim();
        if (category.Length > MaxCategoryLength)
            return Result<DriverLicense>.Failure(DriverErrors.InvalidLicenseCategory);

        // Compare dates, not timestamps: a license stays valid for the
        // whole expiry day. The old check compared against UtcNow and
        // rejected a license expiring later that same day.
        if (expiry.Date < DateTime.UtcNow.Date)
            return Result<DriverLicense>.Failure(DriverErrors.LicenseExpired);

        return Result<DriverLicense>.Success(new DriverLicense(number, expiry, category));
    }

    public bool IsExpired() => ExpiryDate.Date < DateTime.UtcNow.Date;
}

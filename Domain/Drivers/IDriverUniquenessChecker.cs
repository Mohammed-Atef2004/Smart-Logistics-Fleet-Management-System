using Domain.Drivers.ValueObjects;

namespace Domain.Drivers;

/// <summary>
/// Domain service that answers "is this license number still free?".
/// It lives in the Domain next to the aggregate that consumes it; the
/// implementation is infrastructure-specific (a database query).
/// </summary>
public interface IDriverUniquenessChecker
{
    /// <param name="license">License whose number is being checked.</param>
    /// <param name="excludingDriverId">
    /// The driver being updated, so re-saving an unchanged license does
    /// not collide with itself.
    /// </param>
    bool IsLicenseNumberUnique(DriverLicense license, DriverId? excludingDriverId = null);
}

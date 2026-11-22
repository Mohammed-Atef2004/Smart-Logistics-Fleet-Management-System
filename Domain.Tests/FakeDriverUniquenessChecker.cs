using Domain.Drivers;
using Domain.Drivers.ValueObjects;

namespace Domain.Tests;

/// <summary>
/// Configurable driver uniqueness stub. Records the exclusion id so tests
/// can prove the aggregate excludes the driver being updated.
/// </summary>
public sealed class FakeDriverUniquenessChecker : IDriverUniquenessChecker
{
    public FakeDriverUniquenessChecker(bool isUnique = true)
    {
        IsUnique = isUnique;
    }

    public bool IsUnique { get; set; }

    public DriverId? LastExcludedDriverId { get; private set; }

    public bool IsLicenseNumberUnique(DriverLicense license, DriverId? excludingDriverId = null)
    {
        LastExcludedDriverId = excludingDriverId;
        return IsUnique;
    }
}

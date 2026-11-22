using Domain.Drivers;
using Domain.Drivers.ValueObjects;

namespace Application.Tests.Fakes;

/// <summary>
/// Uniqueness stub backed by <see cref="FakeDriverRepository"/>, so handler
/// tests exercise the same "license number already exists" path as the
/// database implementation.
/// </summary>
public sealed class FakeDriverUniquenessChecker : IDriverUniquenessChecker
{
    private readonly FakeDriverRepository _repository;

    public FakeDriverUniquenessChecker(FakeDriverRepository repository)
    {
        _repository = repository;
    }

    public bool IsLicenseNumberUnique(DriverLicense license, DriverId? excludingDriverId = null)
        => !_repository.EntityQuery.Any(d =>
            d.License.LicenseNumber == license.LicenseNumber &&
            (excludingDriverId == null || d.Id != excludingDriverId));
}

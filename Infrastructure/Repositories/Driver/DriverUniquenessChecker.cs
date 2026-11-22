using Domain.Drivers;
using Domain.Drivers.ValueObjects;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// Database-backed implementation of the driver uniqueness domain service.
/// A license number identifies a person, so it must be unique across the
/// fleet; the check excludes the driver being updated.
/// </summary>
public sealed class DriverUniquenessChecker : IDriverUniquenessChecker
{
    private readonly AppDbContext _context;

    public DriverUniquenessChecker(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public bool IsLicenseNumberUnique(DriverLicense license, DriverId? excludingDriverId = null)
    {
        var query = _context.Drivers.AsNoTracking();

        if (excludingDriverId is { } id)
            query = query.Where(d => d.Id != id);

        return !query.Any(d => d.License.LicenseNumber == license.LicenseNumber);
    }
}

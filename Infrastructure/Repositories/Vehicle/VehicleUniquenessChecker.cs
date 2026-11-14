using Application.Common.Interfaces;
using Domain.Vehicles;
using Domain.Vehicles.ValueObjects;
using Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// Implementation of vehicle uniqueness checking
/// </summary>
public sealed class VehicleUniquenessChecker : IVehicleUniquenessChecker
{
    private readonly AppDbContext _context;

    public VehicleUniquenessChecker(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public bool IsPlateUnique(VehiclePlateNumber plateNumber)
    {
        // Synchronous version for domain logic
        return !_context.Vehicles
            .AsNoTracking()
            .Any(v => v.PlateNumber == plateNumber);
    }

    public async Task<bool> IsPlateUniqueAsync(
        VehiclePlateNumber plateNumber,
        CancellationToken cancellationToken = default)
    {
        return !await _context.Vehicles
            .AsNoTracking()
            .AnyAsync(v => v.PlateNumber == plateNumber, cancellationToken);
    }
}

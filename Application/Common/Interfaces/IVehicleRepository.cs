using Domain.Vehicles;
using Domain.Vehicles.Enums;

namespace Application.Common.Interfaces;

/// <summary>
/// Repository contract for the Vehicle aggregate.
/// Inherits <see cref="IVehicleUniquenessChecker"/> (a Domain service) for
/// plate-number uniqueness checks performed by the aggregate itself.
/// </summary>
public interface IVehicleRepository : IGenericRepository<Vehicle>, IVehicleUniquenessChecker
{
    /// <summary>Get vehicle by its strongly-typed ID.</summary>
    Task<Vehicle?> GetByIdAsync(VehicleId id, CancellationToken cancellationToken = default);

    /// <summary>Get vehicle by plate number.</summary>
    Task<Vehicle?> GetByPlateNumberAsync(VehiclePlateNumber plateNumber, CancellationToken cancellationToken = default);

    /// <summary>Check if a vehicle with the given plate number exists.</summary>
    Task<bool> ExistsAsync(VehiclePlateNumber plateNumber, CancellationToken cancellationToken = default);

    /// <summary>Get all available vehicles (ready for assignment).</summary>
    Task<IReadOnlyList<Vehicle>> GetAvailableVehiclesAsync(CancellationToken cancellationToken = default);

    /// <summary>Get vehicles by status.</summary>
    Task<IReadOnlyList<Vehicle>> GetByStatusAsync(VehicleStatus status, CancellationToken cancellationToken = default);

    /// <summary>Get vehicles due for maintenance on a specific date.</summary>
    Task<IReadOnlyList<Vehicle>> GetVehiclesDueForMaintenanceAsync(DateTime date, CancellationToken cancellationToken = default);

    /// <summary>Get vehicles with upcoming maintenance (within the next N days).</summary>
    Task<IReadOnlyList<Vehicle>> GetVehiclesWithUpcomingMaintenanceAsync(int daysAhead = 7, CancellationToken cancellationToken = default);

    /// <summary>Search vehicles by specification criteria (model, year, engine type).</summary>
    Task<IReadOnlyList<Vehicle>> SearchBySpecificationAsync(
        string? model = null,
        int? year = null,
        string? engineType = null,
        CancellationToken cancellationToken = default);

    /// <summary>Get a paginated list of vehicles.</summary>
    Task<(IReadOnlyList<Vehicle> Items, int TotalCount)> GetPagedAsync(
        int pageNumber,
        int pageSize,
        VehicleStatus? status = null,
        CancellationToken cancellationToken = default);
}
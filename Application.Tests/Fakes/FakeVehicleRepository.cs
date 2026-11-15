using Domain.SharedKernel;
using Domain.Vehicles;
using Domain.Vehicles.Enums;
using Domain.Vehicles.ValueObjects;

namespace Application.Tests.Fakes;

/// <summary>
/// In-memory IVehicleRepository for handler tests. Vehicles are stored
/// and returned by reference so tests can assert on aggregate state
/// transitions after a handler runs (same tracking semantics as EF).
/// </summary>
public sealed class FakeVehicleRepository : IVehicleRepository
{
    private readonly Dictionary<Guid, Vehicle> _vehicles = [];

    public List<Vehicle> Added { get; } = [];

    public FakeVehicleRepository(params Vehicle[] seed)
    {
        foreach (var vehicle in seed)
            _vehicles[vehicle.Id.Value] = vehicle;
    }

    public bool IsPlateUnique(VehiclePlateNumber plate)
        => _vehicles.Values.All(v => v.PlateNumber.Value != plate.Value);

    public Task<Vehicle?> GetByIdAsync(VehicleId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_vehicles.GetValueOrDefault(id.Value));

    public Task<Vehicle?> GetByPlateNumberAsync(VehiclePlateNumber plateNumber, CancellationToken cancellationToken = default)
        => Task.FromResult(_vehicles.Values.FirstOrDefault(v => v.PlateNumber.Value == plateNumber.Value));

    public Task<bool> ExistsAsync(VehiclePlateNumber plateNumber, CancellationToken cancellationToken = default)
        => Task.FromResult(_vehicles.Values.Any(v => v.PlateNumber.Value == plateNumber.Value));

    public Task<IReadOnlyList<Vehicle>> GetAvailableVehiclesAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Vehicle>>(_vehicles.Values.Where(v => v.Status == VehicleStatus.Available).ToList());

    public Task<IReadOnlyList<Vehicle>> GetByStatusAsync(VehicleStatus status, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Vehicle>>(_vehicles.Values.Where(v => v.Status == status).ToList());

    public Task<IReadOnlyList<Vehicle>> GetVehiclesDueForMaintenanceAsync(DateTime date, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Vehicle>>(_vehicles.Values
            .Where(v => v.MaintenanceSchedules.Any(m => m.ScheduledDate.Date == date.Date)).ToList());

    public Task<IReadOnlyList<Vehicle>> GetVehiclesWithUpcomingMaintenanceAsync(int daysAhead = 7, CancellationToken cancellationToken = default)
    {
        var start = DateTime.UtcNow.Date;
        var end = start.AddDays(daysAhead);
        return Task.FromResult<IReadOnlyList<Vehicle>>(_vehicles.Values
            .Where(v => v.MaintenanceSchedules.Any(m => m.ScheduledDate >= start && m.ScheduledDate <= end)).ToList());
    }

    public Task<IReadOnlyList<Vehicle>> SearchBySpecificationAsync(
        string? model = null, int? year = null, string? engineType = null, CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<Vehicle>>(_vehicles.Values
            .Where(v =>
                (model is null || v.Specification.Model.Contains(model, StringComparison.OrdinalIgnoreCase)) &&
                (year is null || v.Specification.Year == year) &&
                (engineType is null || v.Specification.EngineType.Contains(engineType, StringComparison.OrdinalIgnoreCase)))
            .ToList());

    public Task<(IReadOnlyList<Vehicle> Items, int TotalCount)> GetPagedAsync(
        int pageNumber, int pageSize, VehicleStatus? status = null, CancellationToken cancellationToken = default)
    {
        var query = _vehicles.Values
            .Where(v => status is null || v.Status == status)
            .OrderBy(v => v.PlateNumber.Value)
            .ToList();
        var items = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        return Task.FromResult<(IReadOnlyList<Vehicle>, int)>((items, query.Count));
    }

    public Task AddAsync(Vehicle entity)
    {
        _vehicles[entity.Id.Value] = entity;
        Added.Add(entity);
        return Task.CompletedTask;
    }
}

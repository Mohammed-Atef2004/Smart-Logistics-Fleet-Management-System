using Domain.Drivers;
using Domain.Drivers.ValueObjects;

namespace Application.Tests.Fakes;

/// <summary>
/// In-memory IDriverRepository for handler tests. Drivers are stored and
/// returned by reference so tests can assert on aggregate state after a
/// handler runs (same tracking semantics as EF Core).
/// </summary>
public sealed class FakeDriverRepository : IDriverRepository
{
    private readonly Dictionary<Guid, Driver> _drivers = [];

    public List<Driver> Added { get; } = [];

    public FakeDriverRepository(params Driver[] seed)
    {
        foreach (var driver in seed)
            _drivers[driver.Id.Value] = driver;
    }

    public IQueryable<Driver> EntityQuery => _drivers.Values.AsQueryable();

    public Task<Driver?> GetByIdAsync(DriverId id, CancellationToken cancellationToken = default)
        => Task.FromResult(_drivers.TryGetValue(id.Value, out var driver) ? driver : null);

    public Task AddAsync(Driver entity)
    {
        _drivers[entity.Id.Value] = entity;
        Added.Add(entity);
        return Task.CompletedTask;
    }

    public void Update(Driver entity) => _drivers[entity.Id.Value] = entity;
}

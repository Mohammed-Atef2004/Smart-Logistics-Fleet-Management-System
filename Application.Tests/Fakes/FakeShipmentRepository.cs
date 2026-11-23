using Domain.Shipments;
using Domain.Shipments.Enums;
using Domain.Shipments.ValueObjects;

namespace Application.Tests.Fakes;

/// <summary>
/// In-memory <see cref="IShipmentRepository"/> for handler tests. Entities are
/// stored and returned by reference so tests can assert on aggregate state
/// after a handler runs (same tracking semantics as EF Core).
/// </summary>
public sealed class FakeShipmentRepository : IShipmentRepository
{
    private readonly Dictionary<Guid, Shipment> _shipments = [];

    /// <summary>Shipments passed to <see cref="AddAsync"/> by the code under test.</summary>
    public List<Shipment> Added { get; } = [];

    public IQueryable<Shipment> EntityQuery => _shipments.Values.AsQueryable();

    public FakeShipmentRepository(params Shipment[] seed)
    {
        foreach (var shipment in seed)
            _shipments[shipment.Id.Value] = shipment;
    }

    /// <summary>Adds a pre-existing shipment without recording it in <see cref="Added"/>.</summary>
    public void Seed(Shipment shipment) => _shipments[shipment.Id.Value] = shipment;

    public Task<Shipment?> GetByIdAsync(ShipmentId id, CancellationToken ct = default) =>
        Task.FromResult(_shipments.TryGetValue(id.Value, out var shipment) ? shipment : null);

    public Task<IReadOnlyList<Shipment>> ListAsync(
        ShipmentStatus? status = null,
        CancellationToken ct = default)
    {
        var query = _shipments.Values.AsEnumerable();

        if (status is { } value)
            query = query.Where(s => s.Status == value);

        IReadOnlyList<Shipment> result = query
            .OrderByDescending(s => s.CreatedAt)
            .ToList();

        return Task.FromResult(result);
    }

    public Task<Shipment?> GetByTrackingNumberAsync(
        string trackingNumber,
        CancellationToken ct = default) =>
        Task.FromResult(_shipments.Values
            .FirstOrDefault(s => s.Tracking.TrackingNumber == trackingNumber));

    public Task<bool> TrackingNumberExistsAsync(
        string trackingNumber,
        CancellationToken ct = default) =>
        Task.FromResult(_shipments.Values
            .Any(s => s.Tracking.TrackingNumber == trackingNumber));

    public Task AddAsync(Shipment entity)
    {
        _shipments[entity.Id.Value] = entity;
        Added.Add(entity);
        return Task.CompletedTask;
    }

    public void Update(Shipment entity) => _shipments[entity.Id.Value] = entity;
}

using Domain.Shipments.Enums;
using Domain.Shipments.ValueObjects;

namespace Domain.Shipments;

/// <summary>
/// Repository contract for the Shipment aggregate.
/// Owned by the Domain (classic DDD); implemented by Infrastructure.
/// </summary>
public interface IShipmentRepository
{
    /// <summary>Deferred query root for read-side composition (AsNoTracking).</summary>
    IQueryable<Shipment> EntityQuery { get; }

    /// <summary>
    /// Tracked lookup for write flows. The generic <see cref="EntityQuery"/>
    /// is AsNoTracking, so handlers that mutate the aggregate must use this
    /// method - otherwise the change is silently discarded on SaveChanges.
    /// </summary>
    Task<Shipment?> GetByIdAsync(ShipmentId id, CancellationToken ct = default);

    /// <summary>
    /// Read-side listing, optionally filtered by status, newest first. Kept on
    /// the repository so the Application layer does not need EF Core.
    /// </summary>
    Task<IReadOnlyList<Shipment>> ListAsync(
        ShipmentStatus? status = null,
        CancellationToken ct = default);

    Task<Shipment?> GetByTrackingNumberAsync(string trackingNumber, CancellationToken ct = default);

    Task<bool> TrackingNumberExistsAsync(string trackingNumber, CancellationToken ct = default);

    Task AddAsync(Shipment entity);

    void Update(Shipment entity);
}

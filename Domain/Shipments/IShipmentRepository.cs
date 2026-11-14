using Domain.Shipments.Enums;

namespace Domain.Shipments;

/// <summary>
/// Repository contract for the Shipment aggregate.
/// Owned by the Domain (classic DDD); implemented by Infrastructure.
/// </summary>
public interface IShipmentRepository
{
    /// <summary>Deferred query root for read-side composition.</summary>
    IQueryable<Shipment> EntityQuery { get; }

    Task AddAsync(Shipment entity);

    Task<Shipment?> GetByTrackingNumberAsync(string trackingNumber, CancellationToken ct = default);
    Task<bool> TrackingNumberExistsAsync(string trackingNumber, CancellationToken ct = default);
}

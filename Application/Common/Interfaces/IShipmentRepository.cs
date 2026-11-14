using Domain.Shipments;
using Domain.Shipments.Enums;

namespace Application.Common.Interfaces;

public interface IShipmentRepository : IGenericRepository<Shipment>
{
    Task<Shipment?> GetByTrackingNumberAsync(string trackingNumber, CancellationToken ct = default);
    Task<bool> TrackingNumberExistsAsync(string trackingNumber, CancellationToken ct = default);
}
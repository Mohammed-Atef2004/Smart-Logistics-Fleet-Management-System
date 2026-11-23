using Domain.SharedKernel;
using Domain.Shipments.ValueObjects;

namespace Domain.Shipments.Events;

/// <summary>Raised when a carrier is assigned to a shipment.</summary>
public sealed record CarrierAssignedEvent(
    ShipmentId ShipmentId,
    string CarrierName,
    DateTime? EstimatedDeliveryDate) : DomainEvent;

namespace Application.Features.Shipment.Commands.AssignCarrier;

/// <summary>Assigns a carrier to a shipment that is still being prepared.</summary>
public sealed record AssignCarrierCommand(
    Guid ShipmentId,
    string CarrierName,
    DateTime? EstimatedDeliveryDate = null) : IRequest<Result>;

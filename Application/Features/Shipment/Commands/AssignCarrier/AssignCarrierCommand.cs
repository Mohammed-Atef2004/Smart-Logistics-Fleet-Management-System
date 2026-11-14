using Domain.Shipments.ValueObjects;

namespace Application.Features.Shipment.Commands.AssignCarrier
{
    public sealed record AssignCarrierCommand(
    ShipmentId ShipmentId,
    string CarrierName,
    DateTime? EstimatedDeliveryDate
) : IRequest<Result>;
}
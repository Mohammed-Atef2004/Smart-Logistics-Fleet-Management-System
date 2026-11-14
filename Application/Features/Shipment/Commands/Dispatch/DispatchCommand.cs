using Domain.Shipments.ValueObjects;

namespace Application.Features.Shipment.Commands.Dispatch
{
    public record DispatchCommand(ShipmentId Id):IRequest<Result>;
}
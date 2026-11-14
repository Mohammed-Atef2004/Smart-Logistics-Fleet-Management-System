using Domain.Shipments.ValueObjects;
using Domain.Shipments.Enums;

namespace Application.Features.Shipment.Commands.AddRoutePoint
{
    public record AddRoutePointCommand(
        ShipmentId Id,
        string location, 
        string description,
        DateTime arrivedAt,
        RoutePointType type
    ) : IRequest<Result>;
}
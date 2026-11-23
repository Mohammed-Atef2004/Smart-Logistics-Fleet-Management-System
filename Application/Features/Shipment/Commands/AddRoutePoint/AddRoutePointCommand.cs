using Domain.Shipments.Enums;

namespace Application.Features.Shipment.Commands.AddRoutePoint;

/// <summary>Records a point the shipment passed through. The id comes from the route.</summary>
public sealed record AddRoutePointCommand(
    Guid ShipmentId,
    string Location,
    string Description,
    DateTime ArrivedAt,
    RoutePointType Type) : IRequest<Result>;

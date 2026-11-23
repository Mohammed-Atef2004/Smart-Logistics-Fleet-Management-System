using Application.Features.Shipment.DTOs;

namespace Application.Features.Shipment.Queries.GetById;

public sealed record GetShipmentByIdQuery(Guid ShipmentId)
    : IRequest<Result<ShipmentDetailsDto>>;

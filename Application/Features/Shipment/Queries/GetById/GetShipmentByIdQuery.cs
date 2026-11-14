using Application.Features.Shipment.DTOs;
using Domain.Shipments.ValueObjects;

namespace Application.Features.Shipment.Queries.GetById
{
    public record GetShipmentByIdQuery
    (
     ShipmentId Id
    ): IRequest<Result<ShipmentDetailsDto>>;
}
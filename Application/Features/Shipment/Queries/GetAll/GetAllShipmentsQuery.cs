using Application.Features.Shipment.DTOs;

namespace Application.Features.Shipment.Queries.GetAll
{
    public record GetAllShipmentsQuery: IRequest<Result<List<ShipmentListDto>>>;
}
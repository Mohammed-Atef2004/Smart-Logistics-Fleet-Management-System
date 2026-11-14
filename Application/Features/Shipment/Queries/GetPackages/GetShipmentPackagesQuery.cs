using Application.Features.Shipment.DTOs;
using Domain.Shipments.ValueObjects;

namespace Application.Features.Shipment.Queries.GetPackages
{
    public record GetShipmentPackagesQuery
    (
        ShipmentId Id
     ):IRequest<Result<List<PackageDto>>>;
}
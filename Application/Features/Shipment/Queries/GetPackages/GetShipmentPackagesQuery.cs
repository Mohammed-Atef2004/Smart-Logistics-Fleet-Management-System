using Application.Features.Shipment.DTOs;

namespace Application.Features.Shipment.Queries.GetPackages;

public sealed record GetShipmentPackagesQuery(Guid ShipmentId)
    : IRequest<Result<List<PackageDto>>>;

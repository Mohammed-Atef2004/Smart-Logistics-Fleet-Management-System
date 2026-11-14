using Domain.Shipments.ValueObjects;

namespace Application.Features.Shipment.Commands.RemovePackage
{
    public sealed record RemovePackageCommand(
    ShipmentId ShipmentId,
    PackageId PackageId
) : IRequest<Result>;
}
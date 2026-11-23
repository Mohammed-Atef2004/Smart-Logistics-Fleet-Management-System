namespace Application.Features.Shipment.Commands.RemovePackage;

/// <summary>Removes a package from a shipment that is still being prepared.</summary>
public sealed record RemovePackageCommand(
    Guid ShipmentId,
    Guid PackageId) : IRequest<Result>;

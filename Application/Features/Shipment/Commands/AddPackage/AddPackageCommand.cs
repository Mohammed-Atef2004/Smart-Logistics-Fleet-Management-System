using Domain.Shipments.ValueObjects;
using Domain.Shipments.Enums;

namespace Application.Features.Shipment.Commands.AddPackage
{
    public sealed record AddPackageCommand(
    ShipmentId ShipmentId,
    string Description,
    decimal WeightValue,
    WeightUnit WeightUnit,
    decimal Length,
    decimal Width,
    decimal Height,
    DimensionUnit DimensionUnit,
    string? Category,
    bool IsFragile,
    bool RequiresRefrigeration,
    decimal DeclaredValue,
    string Currency
) : IRequest<Result<PackageId>>;
}
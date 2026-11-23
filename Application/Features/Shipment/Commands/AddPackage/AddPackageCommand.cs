using Domain.Shipments.Enums;

namespace Application.Features.Shipment.Commands.AddPackage;

/// <summary>Adds a package to an editable shipment. The shipment id comes from the route.</summary>
public sealed record AddPackageCommand(
    Guid ShipmentId,
    string Description,
    decimal WeightValue,
    WeightUnit WeightUnit,
    decimal Length,
    decimal Width,
    decimal Height,
    DimensionUnit DimensionUnit,
    string? Category = null,
    bool IsFragile = false,
    bool RequiresRefrigeration = false,
    decimal DeclaredValue = 0,
    string Currency = "USD") : IRequest<Result<Guid>>;

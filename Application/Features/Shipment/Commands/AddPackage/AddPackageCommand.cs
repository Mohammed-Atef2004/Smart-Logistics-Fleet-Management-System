using Application.Features.Shipments.ValueObjects;
using Domain.SharedKernel;
using Domain.Shipments.Enums;
using Domain.Shipments.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

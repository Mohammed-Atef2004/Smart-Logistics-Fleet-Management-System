using Application.Features.Shipments.ValueObjects;
using Domain.SharedKernel;
using Domain.Shipments.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Shipment.Commands.RemovePackage
{
    public sealed record RemovePackageCommand(
    ShipmentId ShipmentId,
    PackageId PackageId
) : IRequest<Result>;
}

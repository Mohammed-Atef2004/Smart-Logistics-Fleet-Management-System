using Application.Features.Shipment.DTOs;
using Application.Features.Shipments.ValueObjects;
using Domain.SharedKernel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Shipment.Queries.GetPackages
{
    public record GetShipmentPackagesQuery
    (
        ShipmentId Id
     ):IRequest<Result<List<PackageDto>>>;
}

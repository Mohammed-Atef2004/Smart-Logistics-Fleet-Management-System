using Application.Features.Shipments.ValueObjects;
using Domain.SharedKernel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Shipment.Commands.AssignCarrier
{
    public sealed record AssignCarrierCommand(
    ShipmentId ShipmentId,
    string CarrierName,
    DateTime? EstimatedDeliveryDate
) : IRequest<Result>;
}

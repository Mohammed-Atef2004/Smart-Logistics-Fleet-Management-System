using Application.Features.Shipments.ValueObjects;
using Domain.SharedKernel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Shipment.Commands.MarkDelivered
{
    public record MarkShipmentDeliveredCommand
    (
        ShipmentId Id,
        DateTime deliveredAt, 
        string? receivedBy = null
    ): IRequest<Result>;
}

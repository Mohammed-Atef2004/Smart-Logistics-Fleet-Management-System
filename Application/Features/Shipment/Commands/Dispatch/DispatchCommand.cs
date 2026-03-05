using Amazon.Runtime.Internal;
using Application.Features.Shipments.ValueObjects;
using Domain.SharedKernel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Shipment.Commands.Dispatch
{
    public record DispatchCommand(ShipmentId Id):IRequest<Result>;
}

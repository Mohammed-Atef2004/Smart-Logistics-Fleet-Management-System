using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Shipment.Shipment.Commands.CancelShipment
{
    public record CancelShipmentCommand(Guid Id,string reason): IRequest<Unit>;

}

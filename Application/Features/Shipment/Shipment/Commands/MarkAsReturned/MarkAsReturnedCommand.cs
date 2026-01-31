using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Shipment.Shipment.Commands.MarkAsReturned
{
    public record MarkAsReturnedCommand(Guid Id,string readon): IRequest<Unit>;
}

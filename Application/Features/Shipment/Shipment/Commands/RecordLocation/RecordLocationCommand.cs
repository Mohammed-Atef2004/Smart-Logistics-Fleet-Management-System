using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Shipment.Shipment.Commands.RecordLocation
{
    public record RecordLocationCommand(Guid Id,string location,string? notes): IRequest<Unit>;

}

using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Shipment.Shipment.Commands.StartJourney
{
    public record StartJourneyCommand(Guid Id): IRequest<Unit>;
   
}

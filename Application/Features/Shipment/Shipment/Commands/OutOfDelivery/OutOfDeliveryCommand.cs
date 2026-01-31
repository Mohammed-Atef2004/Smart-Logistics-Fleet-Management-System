using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Shipment.Shipment.Commands.OutOfDelivery
{
    public record OutOfDeliveryCommand(Guid Id): IRequest<Unit>;
}

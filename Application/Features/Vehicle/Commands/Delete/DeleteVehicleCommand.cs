using Amazon.Runtime.Internal;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Vehicle.Commands.Delete
{
    public record DeleteVehicleCommand(Guid Id):IRequest<Unit>;

}

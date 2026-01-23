
using Amazon.Runtime.Internal;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Fleet.Driver.Commands.AssignToVehicle
{
    public record DriverAssignToVehicleCommand(Guid Id,Guid VehicleId):IRequest<Unit>;

}

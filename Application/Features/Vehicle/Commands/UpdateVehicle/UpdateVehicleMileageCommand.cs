using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Vehicle.Commands.UpdateVehicle
{
    public record UpdateVehicleMileageCommand(Guid VehicleId, int NewMileage) : IRequest<Unit>;
}

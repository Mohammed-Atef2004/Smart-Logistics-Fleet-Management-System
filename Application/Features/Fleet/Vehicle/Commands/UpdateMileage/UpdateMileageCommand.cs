using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Fleet.Vehicle.Commands.UpdateMileage
{
    public record UpdateMileageCommand(Guid VehicleId, int NewMileage) : IRequest<Unit>;
}

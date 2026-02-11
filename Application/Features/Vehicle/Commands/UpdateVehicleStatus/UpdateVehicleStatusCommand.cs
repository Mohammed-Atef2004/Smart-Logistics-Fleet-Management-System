using Domain.Common;
using Domain.Vehicles.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Vehicle.Commands.UpdateVehicleStatus
{
    public record UpdateVehicleStatusCommand(
    Guid VehicleId,
    VehicleStatus NewStatus) : IRequest<Result>;
}

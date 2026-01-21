using Application.Features.Fleet.Vehicle.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Fleet.Vehicle.Commands.Update
{
    public record UpdateVehicleCommand(Guid VehicleId,UpdateVechicleDto UpdateVechicleDto) : IRequest<Unit>;
}

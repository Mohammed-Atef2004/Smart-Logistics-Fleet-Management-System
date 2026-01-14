using Application.Features.Vehicles.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Vehicle.Queries
{
    public record GetVehicleDetailsQuery(Guid Id) : IRequest<VehicleDto>;
}

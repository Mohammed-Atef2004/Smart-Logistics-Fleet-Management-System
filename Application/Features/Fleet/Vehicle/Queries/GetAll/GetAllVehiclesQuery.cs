using Application.Features.Fleet.Vehicle.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Fleet.Vehicle.Queries.GetAll
{
    public record GetAllVehiclesQuery : IRequest<List<VehicleDto>>
    {
    }   
}

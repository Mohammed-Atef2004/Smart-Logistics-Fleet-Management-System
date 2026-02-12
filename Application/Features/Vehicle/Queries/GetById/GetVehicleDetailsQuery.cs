using Application.Features.Vehicle.DTOs;
using Domain.SharedKernel;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Vehicle.Queries.GetById
{
    public record GetVehicleDetailsQuery(Guid VehicleId) : IRequest<Result<VehicleDetailsDto>>;
}

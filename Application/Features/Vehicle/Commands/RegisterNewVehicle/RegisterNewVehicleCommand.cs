using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.SharedKernel;
using MediatR;

namespace Application.Features.Vehicle.Commands.RegisterNewVehicle
{
    

    public record RegisterNewVehicleCommand(string PlateNumber, VehicleSpecification Specification)
        : IRequest<Result<VehicleId>>;

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Domain;
using Domain.Common;
using MediatR;

namespace Application.Features.Vehicle.Commands.RegisterNewVehicle
{
    

    public record RegisterNewVehicleCommand(VehiclePlateNumber PlateNumber, VehicleSpecification Specification)
        : IRequest<Result<VehicleId>>;

}

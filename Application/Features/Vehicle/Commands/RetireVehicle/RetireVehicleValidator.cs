using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Vehicle.Commands.RetireVehicle
{

    public class RetireVehicleValidator : AbstractValidator<RetireVehicleCommand>
    {
        public RetireVehicleValidator()
        {
            RuleFor(x => x.VehicleId).NotEmpty();
        }
    }
}

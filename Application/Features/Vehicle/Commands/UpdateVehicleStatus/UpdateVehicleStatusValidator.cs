using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Vehicle.Commands.UpdateVehicleStatus
{
    public class UpdateVehicleStatusValidator : AbstractValidator<UpdateVehicleStatusCommand>
    {
        public UpdateVehicleStatusValidator()
        {
            RuleFor(x => x.VehicleId).NotEmpty();

            RuleFor(x => x.NewStatus)
                .IsInEnum()
                .WithMessage("Invalid vehicle status.");
        }
    }
}
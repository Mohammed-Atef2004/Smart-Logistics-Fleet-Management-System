using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Vehicle.Commands.RegisterNewVehicle
{
    public class RegisterNewVehicleCommandValidator : AbstractValidator<RegisterNewVehicleCommand>
    {
        public RegisterNewVehicleCommandValidator()
        {
            RuleFor(x => x.PlateNumber)
                .NotEmpty()
                .WithMessage("Plate number is required");

            RuleFor(x => x.Specification)
                .NotNull()
                .WithMessage("Vehicle specification is required");

            RuleFor(x => x.Specification.Model)
                .NotEmpty()
                .WithMessage("Vehicle model is required");

            RuleFor(x => x.Specification.Year)
                .InclusiveBetween(1900, DateTime.UtcNow.Year + 1)
                .WithMessage("Year must be valid");

            RuleFor(x => x.Specification.EngineType)
                .NotEmpty()
                .WithMessage("Engine type is required");
        }
    }
}

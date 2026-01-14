using Application.Features.Vehicle.Commands.CreateVehicle;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CreateVehicleCommandValidator : AbstractValidator<CreateVehicleCommand>
{
    public CreateVehicleCommandValidator()
    {
        RuleFor(v => v.LicensePlate).NotEmpty().MaximumLength(10);
        RuleFor(v => v.Model).NotEmpty();
        RuleFor(v => v.CurrentMileage).GreaterThanOrEqualTo(0);
    }
}

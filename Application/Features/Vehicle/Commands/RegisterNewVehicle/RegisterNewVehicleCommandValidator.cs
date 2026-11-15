namespace Application.Features.Vehicle.Commands.RegisterNewVehicle;

public class RegisterNewVehicleCommandValidator : AbstractValidator<RegisterNewVehicleCommand>
{
    public RegisterNewVehicleCommandValidator()
    {
        RuleFor(x => x.PlateNumber)
            .NotEmpty()
            .WithMessage("Plate number is required")
            .MaximumLength(20)
            .WithMessage("Plate number cannot exceed 20 characters");

        RuleFor(x => x.Model)
            .NotEmpty()
            .WithMessage("Vehicle model is required")
            .MaximumLength(100);

        RuleFor(x => x.Year)
            .InclusiveBetween(1886, DateTime.UtcNow.Year + 1)
            .WithMessage("Year must be valid");

        RuleFor(x => x.EngineType)
            .NotEmpty()
            .WithMessage("Engine type is required")
            .MaximumLength(50);
    }
}

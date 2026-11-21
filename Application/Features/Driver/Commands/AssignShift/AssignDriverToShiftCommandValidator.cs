namespace Application.Features.Driver.Commands.AssignShift;

public sealed class AssignDriverToShiftCommandValidator : AbstractValidator<AssignDriverToShiftCommand>
{
    public AssignDriverToShiftCommandValidator()
    {
        RuleFor(x => x.DriverId).NotEmpty();

        RuleFor(x => x.Start)
            .NotEmpty()
            .LessThan(x => x.End)
            .WithMessage("Shift start must be before shift end.");

        RuleFor(x => x.End).NotEmpty();
    }
}

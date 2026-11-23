namespace Application.Features.Shift.Commands.Create;

public sealed class CreateShiftCommandValidator : AbstractValidator<CreateShiftCommand>
{
    public CreateShiftCommandValidator()
    {
        RuleFor(x => x.DriverId).NotEmpty();

        RuleFor(x => x.Start)
            .NotEqual(default(DateTime)).WithMessage("Shift start is required.");

        RuleFor(x => x.End)
            .NotEqual(default(DateTime)).WithMessage("Shift end is required.");

        RuleFor(x => x)
            .Must(x => x.End > x.Start)
            .WithMessage("Shift end must be after shift start.")
            .OverridePropertyName(nameof(CreateShiftCommand.End));

        RuleFor(x => x)
            .Must(x => x.End - x.Start <= TimeSpan.FromHours(Domain.Shifts.Shift.MaxDurationHours))
            .WithMessage($"A shift cannot be longer than {Domain.Shifts.Shift.MaxDurationHours} hours.")
            .OverridePropertyName(nameof(CreateShiftCommand.End));
    }
}

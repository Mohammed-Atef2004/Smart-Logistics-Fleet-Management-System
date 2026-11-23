namespace Application.Features.Shift.Commands.StartShift;

public sealed class StartShiftCommandValidator : AbstractValidator<StartShiftCommand>
{
    public StartShiftCommandValidator()
    {
        RuleFor(x => x.ShiftId).NotEmpty();
    }
}

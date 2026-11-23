namespace Application.Features.Shift.Commands.AbortShift;

public sealed class AbortShiftCommandValidator : AbstractValidator<AbortShiftCommand>
{
    public AbortShiftCommandValidator()
    {
        RuleFor(x => x.ShiftId).NotEmpty();
    }
}

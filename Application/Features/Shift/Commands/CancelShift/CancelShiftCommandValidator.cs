namespace Application.Features.Shift.Commands.CancelShift;

public sealed class CancelShiftCommandValidator : AbstractValidator<CancelShiftCommand>
{
    public CancelShiftCommandValidator()
    {
        RuleFor(x => x.ShiftId).NotEmpty();
    }
}

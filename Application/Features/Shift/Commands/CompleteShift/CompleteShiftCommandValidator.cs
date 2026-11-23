namespace Application.Features.Shift.Commands.CompleteShift;

public sealed class CompleteShiftCommandValidator : AbstractValidator<CompleteShiftCommand>
{
    public CompleteShiftCommandValidator()
    {
        RuleFor(x => x.ShiftId).NotEmpty();
    }
}

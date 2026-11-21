namespace Application.Features.Driver.Commands.ClearShift;

public sealed class ClearDriverShiftCommandValidator : AbstractValidator<ClearDriverShiftCommand>
{
    public ClearDriverShiftCommandValidator()
    {
        RuleFor(x => x.DriverId).NotEmpty();
    }
}

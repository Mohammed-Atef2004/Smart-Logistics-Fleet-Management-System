namespace Application.Features.Driver.Commands.Suspend;

public sealed class SuspendDriverCommandValidator : AbstractValidator<SuspendDriverCommand>
{
    public SuspendDriverCommandValidator()
    {
        RuleFor(x => x.DriverId).NotEmpty();
        RuleFor(x => x.Reason).IsInEnum();
    }
}

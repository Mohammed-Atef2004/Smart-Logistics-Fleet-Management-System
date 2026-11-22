namespace Application.Features.Driver.Commands.Terminate;

public sealed class TerminateDriverCommandValidator : AbstractValidator<TerminateDriverCommand>
{
    public TerminateDriverCommandValidator()
    {
        RuleFor(x => x.DriverId).NotEmpty();
    }
}

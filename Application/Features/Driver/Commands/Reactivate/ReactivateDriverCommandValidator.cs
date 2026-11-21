namespace Application.Features.Driver.Commands.Reactivate;

public sealed class ReactivateDriverCommandValidator : AbstractValidator<ReactivateDriverCommand>
{
    public ReactivateDriverCommandValidator()
    {
        RuleFor(x => x.DriverId).NotEmpty();
    }
}

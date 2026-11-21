namespace Application.Features.Driver.Commands.UpdateName;

public sealed class UpdateDriverNameCommandValidator : AbstractValidator<UpdateDriverNameCommand>
{
    public UpdateDriverNameCommandValidator()
    {
        RuleFor(x => x.DriverId).NotEmpty();
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .MaximumLength(Domain.Drivers.Driver.MaxNameLength);
    }
}

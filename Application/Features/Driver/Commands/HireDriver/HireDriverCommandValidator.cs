using Domain.Drivers.ValueObjects;

namespace Application.Features.Driver.Commands.HireDriver;

public sealed class HireDriverCommandValidator : AbstractValidator<HireDriverCommand>
{
    public HireDriverCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required.")
            .MaximumLength(Domain.Drivers.Driver.MaxNameLength)
            .WithMessage($"Full name must not exceed {Domain.Drivers.Driver.MaxNameLength} characters.");

        RuleFor(x => x.LicenseNumber)
            .NotEmpty().WithMessage("License number is required.")
            .MaximumLength(DriverLicense.MaxLicenseNumberLength)
            .WithMessage($"License number must not exceed {DriverLicense.MaxLicenseNumberLength} characters.");

        // Compare dates against UTC (the domain check is UTC-based); the
        // old validator used DateTime.Now, which disagreed near midnight
        // on non-UTC machines.
        RuleFor(x => x.ExpiryDate)
            .Must(date => date.Date >= DateTime.UtcNow.Date)
            .WithMessage("Expiry date must not be in the past.");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Category is required.")
            .MaximumLength(DriverLicense.MaxCategoryLength)
            .WithMessage($"Category must not exceed {DriverLicense.MaxCategoryLength} characters.");
    }
}

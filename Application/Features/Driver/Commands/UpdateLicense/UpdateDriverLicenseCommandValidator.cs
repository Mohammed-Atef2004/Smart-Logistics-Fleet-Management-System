using Domain.Drivers.ValueObjects;

namespace Application.Features.Driver.Commands.UpdateLicense;

public sealed class UpdateDriverLicenseCommandValidator : AbstractValidator<UpdateDriverLicenseCommand>
{
    public UpdateDriverLicenseCommandValidator()
    {
        RuleFor(x => x.DriverId).NotEmpty();

        RuleFor(x => x.LicenseNumber)
            .NotEmpty().WithMessage("License number is required.")
            .MaximumLength(DriverLicense.MaxLicenseNumberLength);

        RuleFor(x => x.ExpiryDate)
            .Must(date => date.Date >= DateTime.UtcNow.Date)
            .WithMessage("Expiry date must not be in the past.");

        RuleFor(x => x.Category)
            .NotEmpty().WithMessage("Category is required.")
            .MaximumLength(DriverLicense.MaxCategoryLength);
    }
}

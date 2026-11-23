using Domain.Shipments.Enums;

namespace Application.Features.Shipment.Commands.AddPackage;

public sealed class AddPackageCommandValidator : AbstractValidator<AddPackageCommand>
{
    public AddPackageCommandValidator()
    {
        RuleFor(x => x.ShipmentId).NotEmpty();

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Package description is required.")
            .MaximumLength(500).WithMessage("Package description cannot exceed 500 characters.");

        RuleFor(x => x.WeightValue)
            .GreaterThan(0).WithMessage("Package weight must be greater than zero.");

        RuleFor(x => x.WeightUnit).IsInEnum();

        RuleFor(x => x.Length).GreaterThan(0).WithMessage("Length must be greater than zero.");
        RuleFor(x => x.Width).GreaterThan(0).WithMessage("Width must be greater than zero.");
        RuleFor(x => x.Height).GreaterThan(0).WithMessage("Height must be greater than zero.");

        RuleFor(x => x.DimensionUnit).IsInEnum();

        RuleFor(x => x.Category)
            .MaximumLength(100).WithMessage("Category cannot exceed 100 characters.");

        RuleFor(x => x.DeclaredValue)
            .GreaterThanOrEqualTo(0).WithMessage("Declared value cannot be negative.");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required.")
            .Length(3).WithMessage("Currency must be a 3-letter ISO code.");
    }
}

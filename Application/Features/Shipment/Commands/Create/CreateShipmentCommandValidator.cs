using Domain.Shipments.Enums;

namespace Application.Features.Shipment.Commands.Create;

public sealed class CreateShipmentCommandValidator : AbstractValidator<CreateShipmentCommand>
{
    public CreateShipmentCommandValidator()
    {
        RuleFor(x => x.SenderId)
            .NotEmpty().WithMessage("Sender ID is required.")
            .MaximumLength(100).WithMessage("Sender ID cannot exceed 100 characters.");

        RuleFor(x => x.Street)
            .NotEmpty().WithMessage("Street is required.")
            .MaximumLength(200).WithMessage("Street cannot exceed 200 characters.");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required.")
            .MaximumLength(100).WithMessage("City cannot exceed 100 characters.");

        RuleFor(x => x.State)
            .NotEmpty().WithMessage("State is required.")
            .MaximumLength(100).WithMessage("State cannot exceed 100 characters.");

        RuleFor(x => x.ZipCode)
            .NotEmpty().WithMessage("Zip code is required.")
            .MaximumLength(20).WithMessage("Zip code cannot exceed 20 characters.");

        RuleFor(x => x.Country)
            .NotEmpty().WithMessage("Country is required.")
            .MaximumLength(100).WithMessage("Country cannot exceed 100 characters.");

        RuleFor(x => x.ApartmentUnit)
            .MaximumLength(50).WithMessage("Apartment/unit cannot exceed 50 characters.");

        RuleFor(x => x.TrackingNumber)
            .NotEmpty().WithMessage("Tracking number is required.")
            .MaximumLength(100).WithMessage("Tracking number cannot exceed 100 characters.");

        RuleFor(x => x.Priority).IsInEnum();

        RuleFor(x => x.RecipientName)
            .MaximumLength(200).WithMessage("Recipient name cannot exceed 200 characters.");

        RuleFor(x => x.RecipientPhone)
            .MaximumLength(50).WithMessage("Recipient phone cannot exceed 50 characters.");

        RuleFor(x => x.SpecialInstructions)
            .MaximumLength(1000).WithMessage("Special instructions cannot exceed 1000 characters.");
    }
}

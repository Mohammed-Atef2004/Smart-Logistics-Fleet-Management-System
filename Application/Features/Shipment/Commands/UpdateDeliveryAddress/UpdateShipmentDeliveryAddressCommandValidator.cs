namespace Application.Features.Shipment.Commands.UpdateDeliveryAddress;

public sealed class UpdateShipmentDeliveryAddressCommandValidator
    : AbstractValidator<UpdateShipmentDeliveryAddressCommand>
{
    public UpdateShipmentDeliveryAddressCommandValidator()
    {
        RuleFor(x => x.ShipmentId).NotEmpty();

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
    }
}

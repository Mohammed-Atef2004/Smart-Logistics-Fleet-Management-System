namespace Application.Features.Shipment.Commands.MarkDeliveryFailed;

public sealed class MarkShipmentDeliveryFailedCommandValidator
    : AbstractValidator<MarkShipmentDeliveryFailedCommand>
{
    public MarkShipmentDeliveryFailedCommandValidator()
    {
        RuleFor(x => x.ShipmentId).NotEmpty();

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Delivery failure reason is required.")
            .MaximumLength(500).WithMessage("Delivery failure reason cannot exceed 500 characters.");
    }
}

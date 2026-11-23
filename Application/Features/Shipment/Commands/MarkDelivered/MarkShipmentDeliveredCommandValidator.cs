namespace Application.Features.Shipment.Commands.MarkDelivered;

public sealed class MarkShipmentDeliveredCommandValidator
    : AbstractValidator<MarkShipmentDeliveredCommand>
{
    public MarkShipmentDeliveredCommandValidator()
    {
        RuleFor(x => x.ShipmentId).NotEmpty();

        RuleFor(x => x.DeliveredAt)
            .NotEqual(default(DateTime)).WithMessage("Delivery timestamp is required.");

        RuleFor(x => x.ReceivedBy)
            .MaximumLength(200).WithMessage("Received by cannot exceed 200 characters.");
    }
}

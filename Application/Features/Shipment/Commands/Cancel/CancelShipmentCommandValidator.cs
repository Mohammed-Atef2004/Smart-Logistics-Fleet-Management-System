namespace Application.Features.Shipment.Commands.Cancel;

public sealed class CancelShipmentCommandValidator : AbstractValidator<CancelShipmentCommand>
{
    public CancelShipmentCommandValidator()
    {
        RuleFor(x => x.ShipmentId).NotEmpty();

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Cancellation reason is required.")
            .MaximumLength(500).WithMessage("Cancellation reason cannot exceed 500 characters.");

        RuleFor(x => x.CancelledBy)
            .NotEmpty().WithMessage("The user cancelling the shipment is required.")
            .MaximumLength(100).WithMessage("Cancelled by cannot exceed 100 characters.");
    }
}

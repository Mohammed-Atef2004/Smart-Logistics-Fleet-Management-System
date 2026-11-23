namespace Application.Features.Shipment.Commands.AssignCarrier;

public sealed class AssignCarrierCommandValidator : AbstractValidator<AssignCarrierCommand>
{
    public AssignCarrierCommandValidator()
    {
        RuleFor(x => x.ShipmentId).NotEmpty();

        RuleFor(x => x.CarrierName)
            .NotEmpty().WithMessage("Carrier name is required.")
            .MaximumLength(200).WithMessage("Carrier name cannot exceed 200 characters.");
    }
}

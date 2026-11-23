namespace Application.Features.Shipment.Commands.AddRoutePoint;

public sealed class AddRoutePointCommandValidator : AbstractValidator<AddRoutePointCommand>
{
    public AddRoutePointCommandValidator()
    {
        RuleFor(x => x.ShipmentId).NotEmpty();

        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("Route point location is required.")
            .MaximumLength(200).WithMessage("Route point location cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Route point description is required.")
            .MaximumLength(500).WithMessage("Route point description cannot exceed 500 characters.");

        RuleFor(x => x.ArrivedAt)
            .NotEqual(default(DateTime)).WithMessage("Arrival time is required.");

        RuleFor(x => x.Type).IsInEnum();
    }
}

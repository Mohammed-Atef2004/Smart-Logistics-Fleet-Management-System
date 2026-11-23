namespace Application.Features.Shipment.Commands.Dispatch;

public sealed class DispatchCommandValidator : AbstractValidator<DispatchCommand>
{
    public DispatchCommandValidator()
    {
        RuleFor(x => x.ShipmentId).NotEmpty();
    }
}

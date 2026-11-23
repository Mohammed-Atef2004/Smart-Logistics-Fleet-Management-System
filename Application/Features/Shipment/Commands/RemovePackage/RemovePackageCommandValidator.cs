namespace Application.Features.Shipment.Commands.RemovePackage;

public sealed class RemovePackageCommandValidator : AbstractValidator<RemovePackageCommand>
{
    public RemovePackageCommandValidator()
    {
        RuleFor(x => x.ShipmentId).NotEmpty();
        RuleFor(x => x.PackageId).NotEmpty();
    }
}

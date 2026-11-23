using Domain.SharedKernel;
using Domain.Shipments.Enums;
using Domain.Shipments.Errors;

namespace Domain.Shipments.Rules;

/// <summary>
/// A shipment is editable only while it is being prepared (Created or
/// ReadyForPickup). Once it is on its way, packages, carrier and address are
/// frozen.
/// </summary>
public sealed class ShipmentMustBeEditableRule : IBusinessRule
{
    private readonly ShipmentStatus _status;

    public ShipmentMustBeEditableRule(ShipmentStatus status) => _status = status;

    public Error Error => ShipmentErrors.NotEditable;

    public bool IsBroken() =>
        _status is not (ShipmentStatus.Created or ShipmentStatus.ReadyForPickup);
}

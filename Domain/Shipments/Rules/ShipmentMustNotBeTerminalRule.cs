using Domain.SharedKernel;
using Domain.Shipments.Errors;
using Domain.Shipments.ValueObjects;

namespace Domain.Shipments.Rules;

/// <summary>Guards operations that are illegal once the shipment is in a terminal state.</summary>
public sealed class ShipmentMustNotBeTerminalRule : IBusinessRule
{
    private readonly TrackingInfo _trackingInfo;

    public ShipmentMustNotBeTerminalRule(TrackingInfo trackingInfo) => _trackingInfo = trackingInfo;

    public Error Error => ShipmentErrors.InvalidStatusTransition;

    public bool IsBroken() => _trackingInfo.IsTerminal;
}

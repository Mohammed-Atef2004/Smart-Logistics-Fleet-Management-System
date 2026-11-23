using Domain.SharedKernel;
using Domain.Shipments.Errors;

namespace Domain.Shipments.Rules;

/// <summary>A shipment cannot be dispatched without a carrier.</summary>
public sealed class ShipmentMustHaveCarrierRule : IBusinessRule
{
    private readonly string? _carrierName;

    public ShipmentMustHaveCarrierRule(string? carrierName) => _carrierName = carrierName;

    public Error Error => ShipmentErrors.EmptyCarrier;

    public bool IsBroken() => string.IsNullOrWhiteSpace(_carrierName);
}

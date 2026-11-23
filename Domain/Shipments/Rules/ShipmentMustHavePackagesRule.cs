using Domain.SharedKernel;
using Domain.Shipments.Errors;

namespace Domain.Shipments.Rules;

/// <summary>A shipment cannot be dispatched without at least one package.</summary>
public sealed class ShipmentMustHavePackagesRule : IBusinessRule
{
    private readonly int _count;

    public ShipmentMustHavePackagesRule(int count) => _count = count;

    public Error Error => ShipmentErrors.NoPackages;

    public bool IsBroken() => _count < 1;
}

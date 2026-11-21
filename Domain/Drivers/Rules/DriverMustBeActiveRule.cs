using Domain.Drivers.Enums;
using Domain.Drivers.Errors;
using Domain.SharedKernel;

namespace Domain.Drivers.Rules;

/// <summary>A driver must be Active to take on new work (e.g. a shift).</summary>
public sealed class DriverMustBeActiveRule : IBusinessRule
{
    private readonly DriverStatus _status;

    public DriverMustBeActiveRule(DriverStatus status) => _status = status;

    public bool IsBroken() => _status != DriverStatus.Active;

    public Error Error => DriverErrors.NotActive;
}

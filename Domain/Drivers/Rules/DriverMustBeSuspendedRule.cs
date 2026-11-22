using Domain.Drivers.Enums;
using Domain.Drivers.Errors;
using Domain.SharedKernel;

namespace Domain.Drivers.Rules;

/// <summary>Only a suspended driver can be reactivated.</summary>
public sealed class DriverMustBeSuspendedRule : IBusinessRule
{
    private readonly DriverStatus _status;

    public DriverMustBeSuspendedRule(DriverStatus status) => _status = status;

    public bool IsBroken() => _status != DriverStatus.Suspended;

    public Error Error => DriverErrors.NotSuspended;
}

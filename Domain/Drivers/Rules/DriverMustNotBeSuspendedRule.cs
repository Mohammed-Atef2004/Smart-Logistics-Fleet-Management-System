using Domain.Drivers.Enums;
using Domain.Drivers.Errors;
using Domain.SharedKernel;

namespace Domain.Drivers.Rules;

/// <summary>A driver who is already suspended cannot be suspended again.</summary>
public sealed class DriverMustNotBeSuspendedRule : IBusinessRule
{
    private readonly DriverStatus _status;

    public DriverMustNotBeSuspendedRule(DriverStatus status) => _status = status;

    public bool IsBroken() => _status == DriverStatus.Suspended;

    public Error Error => DriverErrors.AlreadySuspended;
}

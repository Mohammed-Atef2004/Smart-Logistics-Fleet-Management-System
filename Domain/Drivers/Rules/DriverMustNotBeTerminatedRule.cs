using Domain.Drivers.Enums;
using Domain.Drivers.Errors;
using Domain.SharedKernel;

namespace Domain.Drivers.Rules;

/// <summary>
/// Termination is terminal: no status change is legal once a driver has
/// left the company. This is the guard the old code was missing — a
/// terminated driver could be suspended/reactivated with one call.
/// </summary>
public sealed class DriverMustNotBeTerminatedRule : IBusinessRule
{
    private readonly DriverStatus _status;

    public DriverMustNotBeTerminatedRule(DriverStatus status) => _status = status;

    public bool IsBroken() => _status == DriverStatus.Terminated;

    public Error Error => DriverErrors.Terminated;
}

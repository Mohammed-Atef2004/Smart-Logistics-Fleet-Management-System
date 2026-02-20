using Domain.Drivers.Enums;
using Domain.SharedKernel;

internal class DriverMustNotBeSuspendedRule : IBusinessRule
{
    private readonly DriverStatus _status;
    public DriverMustNotBeSuspendedRule(DriverStatus status)
    {
        _status = status;
    }

    public Error Error => new Error("Driver Status", "Driver must not be suspended.");

    public bool IsBroken()
    {
        return _status == DriverStatus.Suspended;
    }
}
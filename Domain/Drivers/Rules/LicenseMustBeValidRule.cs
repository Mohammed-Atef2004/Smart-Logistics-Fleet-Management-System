using Domain.Drivers.ValueObjects;
using Domain.SharedKernel;

namespace Domain.Drivers.Rules;

internal class LicenseMustBeValidRule : IBusinessRule
{
    private readonly DriverLicense _license;

    public LicenseMustBeValidRule(DriverLicense license)
    {
        _license = license;
    }

    public Error Error => Domain.Drivers.Errors.DriverErrors.LicenseExpired;

    public bool IsBroken() => _license.IsExpired();
}
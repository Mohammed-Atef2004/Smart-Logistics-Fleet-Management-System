using Domain.Drivers.Errors;
using Domain.Drivers.ValueObjects;
using Domain.SharedKernel;

namespace Domain.Drivers.Rules;

/// <summary>A driver cannot work with an expired license.</summary>
public sealed class LicenseMustBeValidRule : IBusinessRule
{
    private readonly DriverLicense _license;

    public LicenseMustBeValidRule(DriverLicense license) => _license = license;

    public Error Error => DriverErrors.LicenseExpired;

    public bool IsBroken() => _license.IsExpired();
}

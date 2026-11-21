using Domain.Drivers.ValueObjects;
using Domain.SharedKernel;

namespace Domain.Drivers.Events;

/// <summary>Raised when a driver's license details are updated.</summary>
public record DriverLicenseUpdatedEvent : DomainEvent
{
    public DriverId DriverId { get; init; }
    public string LicenseNumber { get; init; }
    public DateTime ExpiryDate { get; init; }

    public DriverLicenseUpdatedEvent(DriverId driverId, string licenseNumber, DateTime expiryDate)
    {
        DriverId = driverId;
        LicenseNumber = licenseNumber;
        ExpiryDate = expiryDate;
    }
}

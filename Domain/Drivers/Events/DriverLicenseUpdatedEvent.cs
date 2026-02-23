using Domain.Drivers.ValueObjects;
using Domain.SharedKernel;

public record DriverLicenseUpdatedEvent : DomainEvent
{
    public DriverId Id;

    public DriverLicenseUpdatedEvent(DriverId id)
    {
        Id= id;
    }
}
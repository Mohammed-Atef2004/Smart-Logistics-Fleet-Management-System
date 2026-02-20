using Domain.Drivers.Enums;
using Domain.Drivers.ValueObjects;
using Domain.SharedKernel;

public record DriverSuspendedEvent : DomainEvent
{
    private DriverId Id;
    private DriverSuspensionReason Reason;

    public DriverSuspendedEvent(DriverId id, DriverSuspensionReason reason)
    {
        Id = id;
        Reason = reason;
    }
}
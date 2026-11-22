using Domain.Drivers.Enums;
using Domain.Drivers.ValueObjects;
using Domain.SharedKernel;

namespace Domain.Drivers.Events;

/// <summary>
/// Raised when a driver is suspended. The reason travels in the payload;
/// the old event stored it in a private field, so handlers logged only
/// the event's own GUID.
/// </summary>
public record DriverSuspendedEvent : DomainEvent
{
    public DriverId DriverId { get; init; }
    public DriverSuspensionReason Reason { get; init; }

    public DriverSuspendedEvent(DriverId driverId, DriverSuspensionReason reason)
    {
        DriverId = driverId;
        Reason = reason;
    }
}

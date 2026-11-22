using Domain.Drivers.ValueObjects;
using Domain.SharedKernel;

namespace Domain.Drivers.Events;

/// <summary>
/// Raised when a driver's name changes. It was previously declared but
/// never published (a handler existed for a dead event).
/// </summary>
public record DriverNameUpdatedEvent : DomainEvent
{
    public DriverId DriverId { get; init; }
    public string OldName { get; init; }
    public string NewName { get; init; }

    public DriverNameUpdatedEvent(DriverId driverId, string oldName, string newName)
    {
        DriverId = driverId;
        OldName = oldName;
        NewName = newName;
    }
}

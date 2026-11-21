using Domain.Drivers.ValueObjects;
using Domain.SharedKernel;

namespace Domain.Drivers.Events;

/// <summary>
/// Raised when a new driver is hired. Payload properties are PascalCase
/// and never named "Id" so they cannot shadow <see cref="DomainEvent.Id"/>.
/// </summary>
public record DriverHiredEvent : DomainEvent
{
    public DriverId DriverId { get; init; }
    public string FullName { get; init; }

    public DriverHiredEvent(DriverId driverId, string fullName)
    {
        DriverId = driverId;
        FullName = fullName;
    }
}

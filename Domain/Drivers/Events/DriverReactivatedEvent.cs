using Domain.Drivers.ValueObjects;
using Domain.SharedKernel;

namespace Domain.Drivers.Events;

/// <summary>Raised when a suspended driver is reactivated.</summary>
public record DriverReactivatedEvent : DomainEvent
{
    public DriverId DriverId { get; init; }

    public DriverReactivatedEvent(DriverId driverId)
    {
        DriverId = driverId;
    }
}

using Domain.Drivers.ValueObjects;
using Domain.SharedKernel;

public record DriverReactivatedEvent : DomainEvent
{
    public DriverId Id;

    public DriverReactivatedEvent(DriverId id)
    {
        Id = id;
    }
}
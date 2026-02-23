using Domain.Drivers.ValueObjects;
using Domain.SharedKernel;

namespace Domain.Drivers.Events
{
    public record DriverNameUpdatedEvent : DomainEvent
    {
        public DriverId Id;

        public DriverNameUpdatedEvent(DriverId id)
        {
            Id = id;
        }
    }
}
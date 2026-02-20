using Domain.Drivers.ValueObjects;
using Domain.SharedKernel;

namespace Domain.Drivers.Events
{
    public record DriverHiredEvent: DomainEvent
    {
        public DriverId Id;
        public string Name;
        public DateTime UtcNow;

        public DriverHiredEvent(DriverId id, string name)
        {
            Id = id;
            Name = name;
        }

        public DriverHiredEvent(DriverId id, string name, DateTime utcNow)
        {
            Id = id;
            Name = name;
            UtcNow = utcNow;
        }
    }
}
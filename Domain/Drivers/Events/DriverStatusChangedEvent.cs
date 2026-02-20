using Domain.Drivers.Enums;
using Domain.Drivers.ValueObjects;
using Domain.SharedKernel;

namespace Domain.Drivers.Events
{
    public record DriverStatusChangedEvent : DomainEvent
    {
        public DriverId Id;
        public DriverStatus Inactive;
        public string Reason;

        public DriverStatusChangedEvent(DriverId id, DriverStatus inactive, string reason)
        {
            Id = id;
            Inactive = inactive;
            Reason = reason;
        }
    }
}
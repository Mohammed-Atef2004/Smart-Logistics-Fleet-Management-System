using Domain.Common;

namespace Domain.Fleet.Events
{
    public class DriverStatusChangedEvent : DomainEvent
    {
        public Guid DriverId { get; }
        public bool IsActive { get; }

        public DriverStatusChangedEvent(Guid driverId, bool isActive)
        {
            DriverId = driverId;
            IsActive = isActive;
        }
    }
}
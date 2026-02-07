using Domain.Common;

namespace Domain.Driver.Events
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
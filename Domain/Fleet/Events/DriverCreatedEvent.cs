using Domain.Common;

namespace Domain.Fleet.Events
{
    public class DriverCreatedEvent : DomainEvent
    {
        public Guid DriverId { get; }
        public Guid UserId { get; }
        public string FullName { get; }

        public DriverCreatedEvent(Guid driverId, Guid userId, string fullName)
        {
            DriverId = driverId;
            UserId = userId;
            FullName = fullName;
        }
    }

}
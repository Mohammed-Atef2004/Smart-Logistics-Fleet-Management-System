using Domain.Common;

namespace Domain.Driver.Events
{
    internal class DriverLinkedToUserEvent : DomainEvent
    {
        private Guid id;
        private Guid applicationUserId;

        public DriverLinkedToUserEvent(Guid id, Guid applicationUserId)
        {
            this.id = id;
            this.applicationUserId = applicationUserId;
        }
    }
}
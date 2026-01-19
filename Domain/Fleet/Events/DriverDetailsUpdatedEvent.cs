using Domain.Common;

namespace Domain.Fleet.Events
{
    internal class DriverDetailsUpdatedEvent : DomainEvent
    {
        private Guid id;
        private string fullName;
        private string licenseNumber;

        public DriverDetailsUpdatedEvent(Guid id, string fullName, string licenseNumber)
        {
            this.id = id;
            this.fullName = fullName;
            this.licenseNumber = licenseNumber;
        }
    }
}
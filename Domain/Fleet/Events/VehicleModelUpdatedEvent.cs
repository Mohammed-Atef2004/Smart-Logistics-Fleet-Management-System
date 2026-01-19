using Domain.Common;

namespace Domain.Fleet.Events
{
    internal class VehicleModelUpdatedEvent : DomainEvent
    {
        private Guid id;
        private string model;

        public VehicleModelUpdatedEvent(Guid id, string model)
        {
            this.id = id;
            this.model = model;
        }
    }
}
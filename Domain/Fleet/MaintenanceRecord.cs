using Domain.Common;
using Domain.Enums;
using Domain.Fleet.Events;

namespace Domain.Fleet
{
    public class MaintenanceRecord : BaseEntity
    {
        public Guid VehicleId { get; private set; }
        public virtual Vehicle Vehicle { get; private set; }

        public MaintenanceType Type { get; private set; }
        public string Description { get; private set; }
        public decimal Cost { get; private set; }
        public int MileageAtMaintenance { get; private set; }
        public DateTime MaintenanceDate { get; private set; }

        // Domain Events storage (just like Vehicle)
        private readonly List<DomainEvent> _domainEvents = new();
        public virtual IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        private MaintenanceRecord() { }

        internal MaintenanceRecord(Guid vehicleId, MaintenanceType type, string description, decimal cost, int mileage)
        {
            VehicleId = vehicleId;
            Type = type;
            Description = description;
            Cost = cost;
            MileageAtMaintenance = mileage;
            MaintenanceDate = DateTime.UtcNow;

            // Triggering the Event
            AddDomainEvent(new MaintenancePerformedEvent(this.VehicleId, type, cost));
        }

        protected void AddDomainEvent(DomainEvent domainEvent) => _domainEvents.Add(domainEvent);
        public void ClearDomainEvents() => _domainEvents.Clear();
    }
}
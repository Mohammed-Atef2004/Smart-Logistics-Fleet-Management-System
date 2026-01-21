using Domain.Common;
using Domain.Exceptions;
using Domain.Fleet.Enums;
using Domain.Fleet.Events;
using Domain.Fleet.Rules;

namespace Domain.Fleet.Entities
{
    public class Vehicle : BaseEntity, IAudiatable, ISoftDeletable
    {
        // --- Properties ---
        public string LicensePlate { get; private set; }
        public string Model { get; private set; }
        public VehicleStatus Status { get; private set; }
        public int CurrentMileage { get; private set; }
        public int LastMaintenanceMileage { get; private set; }

        // --- Navigation Properties ---
        // Backing field to encapsulate the collection
        private readonly List<MaintenanceRecord> _maintenanceRecords = new();
        public virtual IReadOnlyCollection<MaintenanceRecord> MaintenanceRecords => _maintenanceRecords.AsReadOnly();

        // --- Auditing & Soft Delete ---
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public string? CreatedBy { get; private set; }
        public string? UpdatedBy { get; private set; }
        public bool IsDeleted { get; private set; }

        // --- Domain Events Storage ---
        private readonly List<DomainEvent> _domainEvents = new();
        public virtual IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        // --- Constructors ---
        private Vehicle() { } // Required for EF Core

        public Vehicle(string licensePlate, string model, int currentMileage)
        {
            LicensePlate = licensePlate;
            Model = model;
            CurrentMileage = currentMileage;
            LastMaintenanceMileage = currentMileage;
            Status = VehicleStatus.Available;

            // Trigger event for new vehicle creation
            AddDomainEvent(new VehicleCreatedEvent(Id, licensePlate));
        }

        // --- Helper for Business Rules ---
        protected static void CheckRule(IBusinessRule rule)
        {
            if (rule.IsBroken())
            {
                throw new BusinessRuleViolationException(rule);
            }
        }

        // --- Business Logic Methods ---

        public void UpdateMileage(int newMileage)
        {
            // Rule: Mileage cannot decrease
            CheckRule(new MileageCannotDecreaseRule(CurrentMileage, newMileage));

            CurrentMileage = newMileage;

            // Rule: Check if maintenance is required (10,000 km interval)
            if (CurrentMileage - LastMaintenanceMileage >= 10000)
            {
                AddDomainEvent(new MaintenanceRequiredEvent(Id, CurrentMileage));
            }
        }

        public void AssignToTrip()
        {
            // Rule: Cannot trip if maintenance is due
            CheckRule(new MaintenanceIntervalRule(CurrentMileage, LastMaintenanceMileage));

            // Rule: Vehicle must be available
            CheckRule(new VehicleMustBeAvailableRule(Status));

            Status = VehicleStatus.OnTrip;
            AddDomainEvent(new VehicleStatusChangedEvent(Id, Status));
        }

        public void AddMaintenanceRecord(MaintenanceType type, string description, decimal cost)
        {
            // Logic to create and link the record
            var record = new MaintenanceRecord(this.Id,type, description, cost, CurrentMileage);
            _maintenanceRecords.Add(record);

            // Update vehicle state
            LastMaintenanceMileage = CurrentMileage;
            Status = VehicleStatus.InMaintenance;

            AddDomainEvent(new VehicleStatusChangedEvent(Id, Status));
        }

        public void CompleteMaintenance()
        {
            Status = VehicleStatus.Available;
            AddDomainEvent(new VehicleStatusChangedEvent(Id, Status));
        }

        // --- Domain Events Management ---
        protected void AddDomainEvent(DomainEvent domainEvent) => _domainEvents.Add(domainEvent);
        public void ClearDomainEvents() => _domainEvents.Clear();

        // --- Auditing & Soft Delete Implementation ---
        public void SetCreated(string user) { CreatedAt = DateTime.UtcNow; CreatedBy = user; }
        public void SetUpdated(string user) { UpdatedAt = DateTime.UtcNow; UpdatedBy = user; }
        public void Delete() => IsDeleted = true;
        public void Restore() => IsDeleted = false;
        public void Create(string licensePlate, string model, int currentMileage)
        {
            LicensePlate = licensePlate;
            Model = model;
            CurrentMileage = currentMileage;
            LastMaintenanceMileage = currentMileage;
            Status = VehicleStatus.Available;
            AddDomainEvent(new VehicleCreatedEvent(Id, licensePlate));
        }
        public void Update(string licensePlate, string model, int currentMileage)
        {
            LicensePlate = licensePlate;
            Model = model;
            CurrentMileage = currentMileage;
            AddDomainEvent(new VehicleUpdatedEvent(Id, licensePlate));
        }
    }
}

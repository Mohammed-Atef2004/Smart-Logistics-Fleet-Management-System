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

        // --- Driver Relationship (The Missing Link) ---
        public Guid? CurrentDriverId { get; private set; }
        public virtual Driver? CurrentDriver { get; private set; }

        // --- Navigation Properties ---
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

            AddDomainEvent(new VehicleCreatedEvent(Id, licensePlate));
        }

        // --- Helper for Business Rules ---
        protected static void CheckRule(IBusinessRule rule)
        {
            if (rule.IsBroken()) throw new BusinessRuleViolationException(rule);
        }

        // --- Business Logic Methods ---

        // 1. الربط مع السائق
        public void AssignDriver(Guid driverId)
        {
            // لا يمكن تعيين سائق إذا كانت السيارة في الصيانة
            if (Status == VehicleStatus.InMaintenance)
                throw new Exception("Cannot assign driver to a vehicle in maintenance.");

            // إذا كانت السيارة مع سائق آخر بالفعل
            if (CurrentDriverId.HasValue && CurrentDriverId != driverId)
                throw new Exception("Vehicle is already assigned to another driver.");

            CurrentDriverId = driverId;
            Status = VehicleStatus.Active; // تغيير الحالة لنشطة

            AddDomainEvent(new VehicleStatusChangedEvent(Id, Status));
        }

        public void ReleaseDriver()
        {
            CurrentDriverId = null;
            Status = VehicleStatus.Available;
            AddDomainEvent(new VehicleStatusChangedEvent(Id, Status));
        }

        public void UpdateMileage(int newMileage)
        {
            CheckRule(new MileageCannotDecreaseRule(CurrentMileage, newMileage));
            CurrentMileage = newMileage;

            if (CurrentMileage - LastMaintenanceMileage >= 10000)
            {
                AddDomainEvent(new MaintenanceRequiredEvent(Id, CurrentMileage));
            }
        }

        public void AssignToTrip()
        {
            CheckRule(new MaintenanceIntervalRule(CurrentMileage, LastMaintenanceMileage));
            CheckRule(new VehicleMustBeAvailableRule(Status));

            Status = VehicleStatus.OnTrip;
            AddDomainEvent(new VehicleStatusChangedEvent(Id, Status));
        }

        public void AddMaintenanceRecord(MaintenanceType type, string description, decimal cost)
        {
            var record = new MaintenanceRecord(this.Id, type, description, cost, CurrentMileage);
            _maintenanceRecords.Add(record);

            LastMaintenanceMileage = CurrentMileage;
            Status = VehicleStatus.InMaintenance;

            ReleaseDriver();

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

        public void Update(string licensePlate, string model, int currentMileage)
        {
            LicensePlate = licensePlate;
            Model = model;
            CurrentMileage = currentMileage;
            AddDomainEvent(new VehicleUpdatedEvent(Id, licensePlate));
        }
    }
}
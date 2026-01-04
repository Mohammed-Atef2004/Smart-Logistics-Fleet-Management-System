using Domain.Common;
using Domain.Exceptions;
using Domain.Fleet.Events;
using Domain.Fleet.Rules;
using Domain.Users;

namespace Domain.Fleet
{
    public class Driver : BaseEntity, IAudiatable, ISoftDeletable
    {
        // 1. Identity Link
        public Guid UserId { get; private set; } // Link to ApplicationUser (Identity Module)

        // 2. Properties
        public string FullName { get; private set; }
        public string LicenseNumber { get; private set; }
        public bool IsActive { get; private set; }
        public Guid? ApplicationUserId { get; private set; } // Optional link to ApplicationUser
        public virtual ApplicationUser? ApplicationUser { get; private set; }

        // 3. Navigation Property (The relationship with Vehicle)
        // This allows us to know which vehicle the driver is currently assigned to
        public Guid? CurrentVehicleId { get; private set; }
        public virtual Vehicle? CurrentVehicle { get; private set; }

        // 4. Auditing & Soft Delete
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public string? CreatedBy { get; private set; }
        public string? UpdatedBy { get; private set; }
        public bool IsDeleted { get; private set; }

        // 5. Domain Events Storage
        private readonly List<DomainEvent> _domainEvents = new();
        public virtual IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        private Driver() { } // Required for EF Core

        public Driver(Guid userId, string fullName, string licenseNumber)
        {
            // Business Rules Validation
            CheckRule(new DriverNameMustBeValidRule(fullName));
            CheckRule(new LicenseNumberMustBeValidRule(licenseNumber));

            UserId = userId;
            FullName = fullName;
            LicenseNumber = licenseNumber;
            IsActive = true;

            // Trigger Event: To notify that a new driver has joined
            AddDomainEvent(new DriverCreatedEvent(this.Id, userId, fullName));
        }

        // --- Business Logic Methods ---

        public void AssignToVehicle(Guid vehicleId)
        {
            // Rule: Driver must be active to be assigned to a vehicle
            if (!IsActive)
                throw new BusinessRuleViolationException(new DriverMustBeActiveRule(IsActive));

            CurrentVehicleId = vehicleId;

            // Trigger Event: Useful for the Trip module
            AddDomainEvent(new DriverAssignedToVehicleEvent(Id, vehicleId));
        }

        public void UnassignFromVehicle()
        {
            CurrentVehicleId = null;
        }

        public void Deactivate()
        {
            IsActive = false;
            AddDomainEvent(new DriverStatusChangedEvent(Id, false));
        }

        public void Activate()
        {
            IsActive = true;
            AddDomainEvent(new DriverStatusChangedEvent(Id, true));
        }

        // --- Helper Methods ---
        protected static void CheckRule(IBusinessRule rule)
        {
            if (rule.IsBroken()) throw new BusinessRuleViolationException(rule);
        }

        protected void AddDomainEvent(DomainEvent domainEvent) => _domainEvents.Add(domainEvent);
        public void ClearDomainEvents() => _domainEvents.Clear();

        // --- Auditing & Soft Delete Implementation ---
        public void SetCreated(string user) { CreatedAt = DateTime.UtcNow; CreatedBy = user; }
        public void SetUpdated(string user) { UpdatedAt = DateTime.UtcNow; UpdatedBy = user; }
        public void Delete() => IsDeleted = true;
        public void Restore() => IsDeleted = false;
    }
}
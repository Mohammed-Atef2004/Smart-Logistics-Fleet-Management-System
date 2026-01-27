using Domain.Common;
using Domain.Exceptions;
using Domain.Fleet.Events;
using Domain.Fleet.Rules;
using Domain.Users;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Fleet.Entities
{
    public class Driver : BaseEntity, IAudiatable, ISoftDeletable
    {
        // 1. Identity Link (Foreign Key)
        // هذا الحقل هو الربط الأساسي مع جدول المستخدمين
        public Guid Id { get; private set; }
       
        // 2. Properties
        public string FullName { get; private set; }
        public string LicenseNumber { get; private set; }
        public bool IsActive { get; private set; }

        // 3. Navigation Property (The relationship with Vehicle)
        [NotMapped]
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

        // Required for EF Core
        private Driver() { }

        // 6. Primary Constructor
        public Driver(string fullName, string licenseNumber)
        {
            // Business Rules Validation
            CheckRule(new DriverNameMustBeValidRule(fullName));
            CheckRule(new LicenseNumberMustBeValidRule(licenseNumber));

            Id= Guid.NewGuid();
            FullName = fullName;
            LicenseNumber = licenseNumber;
            IsActive = true;

            // Trigger Event: مررنا الـ userId للحدث أيضاً
            AddDomainEvent(new DriverCreatedEvent(Id, fullName,licenseNumber));
        }

        // --- Business Logic Methods ---

        public void Update(string fullName, string licenseNumber)
        {
            CheckRule(new DriverNameMustBeValidRule(fullName));
            CheckRule(new LicenseNumberMustBeValidRule(licenseNumber));

            FullName = fullName;
            LicenseNumber = licenseNumber;

            AddDomainEvent(new DriverDetailsUpdatedEvent(Id, fullName, licenseNumber));
        }

        public void AssignToVehicle(Guid vehicleId)
        {
            if (!IsActive)
                throw new BusinessRuleViolationException(new DriverMustBeActiveRule(IsActive));

            CurrentVehicleId = vehicleId;
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
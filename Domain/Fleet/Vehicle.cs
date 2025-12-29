using Domain.Common;
using Domain.Enums;
using System;

namespace Domain.Fleet
{
    public class Vehicle : BaseEntity, IAudiatable, ISoftDeletable
    {
        public string LicensePlate { get; private set; }
        public string Model { get; private set; }
        public VehicleStatus Status { get; private set; } = VehicleStatus.Available;

        // Auditing
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public string? CreatedBy { get; private set; }
        public string? UpdatedBy { get; private set; }

        // Soft delete
        public bool IsDeleted { get; private set; }

        private Vehicle() { } // For EF

        public Vehicle(string licensePlate, string model)
        {
            LicensePlate = licensePlate;
            Model = model;
            Status = VehicleStatus.Available;
        }

        // Auditing methods
        public void SetCreated(string user)
        {
            CreatedAt = DateTime.UtcNow;
            CreatedBy = user;
        }

        public void SetUpdated(string user)
        {
            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = user;
        }

        // Soft delete methods
        public void Delete() => IsDeleted = true;
        public void Restore() => IsDeleted = false;

        // Status management
        public void ChangeStatus(VehicleStatus newStatus)
        {
            Status = newStatus;
            AddDomainEvent(new VehicleStatusChangedEvent(Id, newStatus));
        }

        private void AddDomainEvent(object domainEvent)
        {
            // سيتم تخزين الأحداث هنا ليتم نشرها لاحقًا في Application Layer
        }
    }
}

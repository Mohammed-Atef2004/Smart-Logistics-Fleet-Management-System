using Domain.Common;
using Domain.Users;
using System;

namespace Domain.Fleet
{
    public class Driver : BaseEntity, IAudiatable, ISoftDeletable
    {
        public string Name { get; private set; }
        public string LicenseNumber { get; private set; }

        // ✅ صححت الـ typo والـ type
        public Guid ApplicationUserId { get; private set; }
        public ApplicationUser ApplicationUser { get; private set; }

        // Relationship
        public Vehicle? Vehicle { get; private set; }

        internal void AssignVehicle(Vehicle vehicle)
        {
            Vehicle = vehicle;
        }

        internal void RemoveVehicle()
        {
            Vehicle = null;
        }

        // Auditable
        public DateTime CreatedAt { get; private set; }
        public string? CreatedBy { get; private set; }
        public DateTime? UpdatedAt { get; private set; }
        public string? UpdatedBy { get; private set; }

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

        // Soft Delete
        public bool IsDeleted { get; private set; } = false;
        public void Delete() => IsDeleted = true;
        public void Restore() => IsDeleted = false;

        // Constructor
        private Driver() { } // ✅ للـ EF Core

        public Driver(string name, string licenseNumber, Guid applicationUserId)
        {
            Name = name;
            LicenseNumber = licenseNumber;
            ApplicationUserId = applicationUserId;
        }
    }
}
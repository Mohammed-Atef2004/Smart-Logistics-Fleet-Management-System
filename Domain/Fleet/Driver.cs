using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Fleet
{
    public class Driver : BaseEntity, IAudiatable, ISoftDeletable
    {
        public string Name { get; private set; }
        public string LicenseNumber { get; private set; }

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

        public void SetCreated(string user) => CreatedAt = DateTime.UtcNow;
        public void SetUpdated(string user) => UpdatedAt = DateTime.UtcNow;

        // Soft Delete
        public bool IsDeleted { get; private set; } = false;
        public void Delete() => IsDeleted = true;
        public void Restore() => IsDeleted = false;

        // Constructor
        public Driver(string name, string licenseNumber)
        {
            Name = name;
            LicenseNumber = licenseNumber;
        }
    }


}

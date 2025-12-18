using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Fleet
{
    public class MaintenanceRecord : BaseEntity
    {
        public DateTime MaintenanceDate { get; private set; }
        public string Description { get; private set; }
        public decimal Cost { get; private set; }

        // Relationship
        public Guid VehicleId { get; private set; }
        public Vehicle Vehicle { get; private set; }

        public MaintenanceRecord(DateTime maintenanceDate, string description, decimal cost, Vehicle vehicle)
        {
            MaintenanceDate = maintenanceDate;
            Description = description;
            Cost = cost;
            Vehicle = vehicle;
            VehicleId = vehicle.Id;
        }
    }

}

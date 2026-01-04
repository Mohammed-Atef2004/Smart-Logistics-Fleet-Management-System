using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Domain.Common;

namespace Domain.Fleet.Events
{
    public class DriverAssignedToVehicleEvent : DomainEvent
    {
        public Guid DriverId { get; }
        public Guid VehicleId { get; }

        public DriverAssignedToVehicleEvent(Guid driverId, Guid vehicleId)
        {
            DriverId = driverId;
            VehicleId = vehicleId;
        }
    }
}
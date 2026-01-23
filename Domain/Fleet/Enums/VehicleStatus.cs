using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Fleet.Enums
{
    public enum VehicleStatus
    {
        Available = 1,
        OnTrip = 2,
        InMaintenance = 3,
        OutOfService = 4,
        Active = 5
    }

}

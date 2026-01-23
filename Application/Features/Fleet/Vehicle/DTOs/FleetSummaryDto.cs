using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Fleet.Vehicle.DTOs
{
    public record FleetSummaryDto(
    int TotalVehicles,
    int VehiclesInMaintenance,
    int VehiclesOnTrip,
    int TotalDrivers,
    int AvailableDrivers
    );
}

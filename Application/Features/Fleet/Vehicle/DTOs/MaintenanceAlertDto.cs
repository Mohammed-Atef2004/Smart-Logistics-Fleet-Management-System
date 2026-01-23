using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Fleet.Vehicle.DTOs
{
    public record MaintenanceAlertDto(
    Guid VehicleId,
    string LicensePlate,
    int CurrentMileage,
    int LastMaintenanceMileage,
    int OverdueByKm 
);
}

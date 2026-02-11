using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Vehicle.DTOs
{
    public record VehicleDetailsDto(
        Guid Id,
        string PlateNumber,
        string Status,
        string Model,
        int Year,
        DateTime? LastMaintenanceDate);
}

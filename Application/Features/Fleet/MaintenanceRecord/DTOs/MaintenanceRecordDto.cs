using Domain.Fleet.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Fleet.MaintenanceRecord.DTOs;

public class MaintenanceRecordDto
{
    public Guid VehicleId { get; init; }
    public int Type { get; init; }

    public string Description { get; init; }
    public decimal Cost { get; init; }
    public int MileageAtMaintenance { get; init; }
}  


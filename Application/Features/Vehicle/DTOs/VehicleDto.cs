using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Vehicles.DTOs;

public record VehicleDto
{
    public string LicensePlate { get; init; } = default!;
    public string Model { get; init; } = default!;
    public string Status { get; init; } = default!;
    public int CurrentMileage { get; init; }
    public bool IsMaintenanceDue { get; init; } 
}

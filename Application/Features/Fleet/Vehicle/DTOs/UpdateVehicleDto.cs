using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Fleet.Vehicle.DTOs;

public class UpdateVechicleDto
{

    public string Model { get; init; } = default!;
    public int CurrentMileage { get; init; }
}

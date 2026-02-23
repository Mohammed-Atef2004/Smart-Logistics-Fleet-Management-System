using Domain.Drivers.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Driver.DTOs
{
    public record DriverDto(
    DriverId Id,
    string FullName,
    string Status,
    double Rating
);
}

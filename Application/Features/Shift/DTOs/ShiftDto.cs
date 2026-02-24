using Domain.Shifts.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Shift.DTOs
{
    public record ShiftDto(ShiftId Id, Guid DriverId, DateTime Start, DateTime End, string Status);
}

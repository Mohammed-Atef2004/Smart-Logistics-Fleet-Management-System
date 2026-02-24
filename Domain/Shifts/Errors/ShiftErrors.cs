using Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Shifts.Errors
{
    public static class ShiftErrors
    {
        public static Error InvalidDuration =
            new("Shift.InvalidDuration", "Start must be before End");

        public static Error InvalidState =
            new("Shift.InvalidState", "Invalid shift state transition");

        public static Error ActiveShiftExists =
            new("Shift.ActiveExists", "Driver already has active shift");
    }
}

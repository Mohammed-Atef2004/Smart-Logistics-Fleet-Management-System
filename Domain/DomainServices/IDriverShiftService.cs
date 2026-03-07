using Domain.Drivers.ValueObjects;
using Domain.SharedKernel;
using Domain.Shifts;
using Domain.Shifts.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainDomainServices
{
    public interface IDriverShiftService
    {
        Task<Result<Shift>> AssignDriverToShift(DriverId driverId, DateTime start, DateTime end);
        Task<Result> StartShift(ShiftId shiftId);
    }
}

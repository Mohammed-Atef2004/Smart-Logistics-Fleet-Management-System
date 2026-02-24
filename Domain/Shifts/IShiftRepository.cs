using Domain.Drivers.ValueObjects;
using Domain.Interfaces.Repositories;
using Domain.Shifts.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Shifts
{
    public interface IShiftRepository:IGenericRepository<Shift>
    {
        Task<bool> HasActiveShift(DriverId driverId);
        Task<bool> HasOverlappingShift(DriverId driverId, DateTime start, DateTime end);

    }
}

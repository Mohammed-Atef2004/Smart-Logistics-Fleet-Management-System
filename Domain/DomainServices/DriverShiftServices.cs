using Domain.Drivers;
using Domain.Drivers.Enums;
using Domain.Drivers.ValueObjects;
using Domain.SharedKernel;
using Domain.Shifts;
using Domain.Shifts.ValueObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DomainServices
{
    public sealed class DriverShiftService : IDriverShiftService
    {
        private readonly IDriverRepository _drivers;
        private readonly IShiftRepository _shifts;

        public DriverShiftService(IDriverRepository drivers, IShiftRepository shifts)
        {
            _drivers = drivers;
            _shifts = shifts;
        }

        // ✅ Only cross-aggregate logic
        public async Task<Result<Shift>> AssignDriverToShift(DriverId driverId, DateTime start, DateTime end)
        {
            var driver = await _drivers.EntityQuery.SingleOrDefaultAsync(x=>x.Id==driverId);
            if (driver is null || driver.Status != DriverStatus.Active)
                return Result<Shift>.Failure(new("Driver.Invalid", "Driver not active"));

            if (await _shifts.HasOverlappingShift(driverId, start, end))
                return Result<Shift>.Failure(new("Shift.Overlap", "Driver already has shift"));

            var shiftResult = Shift.Create(driverId, start, end);
            if (shiftResult.IsFailure) return shiftResult;

            await _shifts.AddAsync(shiftResult.Value);
            return Result<Shift>.Success(shiftResult.Value);
        }

        public async Task<Result> StartShift(ShiftId shiftId)
        {
            var shift = await _shifts.EntityQuery.SingleOrDefaultAsync(x=>x.Id==shiftId);
            if (shift is null) return Result.Failure(new("Shift.NotFound", "Not found"));

            var driver = await _drivers.EntityQuery.SingleOrDefaultAsync(x=>x.Id==shift.DriverId);
            if (driver is null || driver.Status != DriverStatus.Active)
                return Result.Failure(new("Driver.Invalid", "Driver not active"));

            var result = shift.Start();
            if (result.IsFailure) return result;

             _shifts.Update(shift);
            return Result.Success();
        }
    }
}

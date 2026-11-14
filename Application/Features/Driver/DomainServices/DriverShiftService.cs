using Application.Common.Interfaces;
using Domain.Drivers;
using Domain.Drivers.Enums;
using Domain.Drivers.ValueObjects;
using Domain.SharedKernel;
using Domain.Shifts;
using Domain.Shifts.ValueObjects;
using Microsoft.EntityFrameworkCore;
using ShiftAggregate = Domain.Shifts.Shift;

namespace Application.Features.Driver.DomainServices;

/// <summary>
/// Cross-aggregate coordination between Driver and Shift.
/// Lives in the Application layer because it orchestrates two aggregates
/// and depends on queryable repository access â€” it is not pure domain logic.
/// </summary>
public sealed class DriverShiftService : IDriverShiftService
{
    private readonly IDriverRepository _drivers;
    private readonly IShiftRepository _shifts;

    public DriverShiftService(IDriverRepository drivers, IShiftRepository shifts)
    {
        _drivers = drivers;
        _shifts = shifts;
    }

    // âœ… Only cross-aggregate logic
    public async Task<Result<ShiftAggregate>> AssignDriverToShift(DriverId driverId, DateTime start, DateTime end)
    {
        var driver = await _drivers.EntityQuery.SingleOrDefaultAsync(x => x.Id == driverId);
        if (driver is null || driver.Status != DriverStatus.Active)
            return Result<ShiftAggregate>.Failure(new("Driver.Invalid", "Driver not active"));

        if (await _shifts.HasOverlappingShift(driverId, start, end))
            return Result<ShiftAggregate>.Failure(new("Shift.Overlap", "Driver already has shift"));

        var shiftResult = ShiftAggregate.Create(driverId, start, end);
        if (shiftResult.IsFailure) return shiftResult;

        await _shifts.AddAsync(shiftResult.Value);
        return Result<ShiftAggregate>.Success(shiftResult.Value);
    }

    public async Task<Result> StartShift(ShiftId shiftId)
    {
        var shift = await _shifts.EntityQuery.SingleOrDefaultAsync(x => x.Id == shiftId);
        if (shift is null) return Result.Failure(new("Shift.NotFound", "Not found"));

        var driver = await _drivers.EntityQuery.SingleOrDefaultAsync(x => x.Id == shift.DriverId);
        if (driver is null || driver.Status != DriverStatus.Active)
            return Result.Failure(new("Driver.Invalid", "Driver not active"));

        var result = shift.Start();
        if (result.IsFailure) return result;

        _shifts.Update(shift);
        return Result.Success();
    }
}

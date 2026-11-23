using Domain.Drivers;
using Domain.Drivers.Enums;
using Domain.Drivers.Errors;
using Domain.Drivers.ValueObjects;
using Domain.SharedKernel;
using Domain.Shifts;
using Domain.Shifts.Errors;
using ShiftAggregate = Domain.Shifts.Shift;

namespace Application.Features.Driver.DomainServices;

public sealed class DriverShiftService : IDriverShiftService
{
    private readonly IDriverRepository _drivers;
    private readonly IShiftRepository _shifts;

    public DriverShiftService(IDriverRepository drivers, IShiftRepository shifts)
    {
        _drivers = drivers;
        _shifts = shifts;
    }

    /// <summary>
    /// Creates the shift and marks it as the driver's current shift.
    /// The old implementation created the shift but never called
    /// <c>driver.AssignShift</c>, so <c>CurrentShiftId</c> stayed null
    /// (the driver still looked available) and the domain rules were
    /// never enforced. <c>StartShift</c> was a duplicate of the Shift
    /// module's own handler and was removed as dead code.
    /// </summary>
    public async Task<Result<ShiftAggregate>> AssignDriverToShift(
        DriverId driverId, DateTime start, DateTime end)
    {
        var driver = await _drivers.GetByIdAsync(driverId);
        if (driver is null)
            return Result<ShiftAggregate>.Failure(DriverErrors.NotFound(driverId.Value));

        if (driver.Status != DriverStatus.Active)
            return Result<ShiftAggregate>.Failure(DriverErrors.NotActive);

        if (driver.CurrentShiftId is not null)
            return Result<ShiftAggregate>.Failure(DriverErrors.AlreadyInShift);

        if (await _shifts.HasOverlappingShift(driverId, start, end))
            return Result<ShiftAggregate>.Failure(ShiftErrors.Overlap);

        var shiftResult = ShiftAggregate.Create(driverId, start, end);
        if (shiftResult.IsFailure)
            return Result<ShiftAggregate>.Failure(shiftResult.Error);

        var assignResult = driver.AssignShift(shiftResult.Value.Id);
        if (assignResult.IsFailure)
            return Result<ShiftAggregate>.Failure(assignResult.Error);

        _drivers.Update(driver);
        await _shifts.AddAsync(shiftResult.Value);

        return Result<ShiftAggregate>.Success(shiftResult.Value);
    }
}

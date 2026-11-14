using Domain.Drivers.ValueObjects;
using Domain.Shifts;
using Domain.Shifts.ValueObjects;
using ShiftAggregate = Domain.Shifts.Shift;

namespace Application.Features.Driver.DomainServices;

public interface IDriverShiftService
{
    Task<Result<ShiftAggregate>> AssignDriverToShift(DriverId driverId, DateTime start, DateTime end);
    Task<Result> StartShift(ShiftId shiftId);
}
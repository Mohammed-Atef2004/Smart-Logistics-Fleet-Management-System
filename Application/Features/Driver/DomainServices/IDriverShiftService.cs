using Domain.Drivers;
using Domain.Drivers.Errors;
using Domain.Drivers.ValueObjects;
using Domain.SharedKernel;
using Domain.Shifts;
using ShiftAggregate = Domain.Shifts.Shift;

namespace Application.Features.Driver.DomainServices;

/// <summary>
/// Cross-aggregate coordination between Driver and Shift. It lives in
/// the Application layer because it orchestrates two aggregates and
/// depends on queryable repositories — it is not pure domain logic.
/// </summary>
public interface IDriverShiftService
{
    Task<Result<ShiftAggregate>> AssignDriverToShift(DriverId driverId, DateTime start, DateTime end);
}

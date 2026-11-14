using Domain.Drivers.ValueObjects;
using Domain.Shifts;
using Domain.Shifts.ValueObjects;

namespace Application.Common.Interfaces;

public interface IShiftRepository : IGenericRepository<Shift>
{
    Task<bool> HasActiveShift(DriverId driverId);
    Task<bool> HasOverlappingShift(DriverId driverId, DateTime start, DateTime end);
}
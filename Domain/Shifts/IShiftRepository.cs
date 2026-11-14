using Domain.Drivers.ValueObjects;
using Domain.Shifts.ValueObjects;

namespace Domain.Shifts;

/// <summary>
/// Repository contract for the Shift aggregate.
/// Owned by the Domain (classic DDD); implemented by Infrastructure.
/// </summary>
public interface IShiftRepository
{
    /// <summary>Deferred query root for read-side composition.</summary>
    IQueryable<Shift> EntityQuery { get; }

    Task AddAsync(Shift entity);
    void Update(Shift entity);

    Task<bool> HasActiveShift(DriverId driverId);
    Task<bool> HasOverlappingShift(DriverId driverId, DateTime start, DateTime end);
}

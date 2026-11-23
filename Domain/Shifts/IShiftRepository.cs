using Domain.Drivers.ValueObjects;
using Domain.Shifts.Enums;
using Domain.Shifts.ValueObjects;

namespace Domain.Shifts;

/// <summary>
/// Repository contract for the Shift aggregate.
/// Owned by the Domain (classic DDD); implemented by Infrastructure.
/// </summary>
public interface IShiftRepository
{
    /// <summary>Deferred query root for read-side composition (AsNoTracking).</summary>
    IQueryable<Shift> EntityQuery { get; }

    /// <summary>
    /// Tracked lookup for write flows. The generic <see cref="EntityQuery"/>
    /// is AsNoTracking, so handlers that mutate the aggregate must use this
    /// method - otherwise the change is silently discarded on SaveChanges.
    /// </summary>
    Task<Shift?> GetByIdAsync(ShiftId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Read-side listing with optional filters, ordered by start time.
    /// Kept on the repository so the Application layer does not have to
    /// depend on EF Core's async materialization to page through shifts.
    /// </summary>
    Task<IReadOnlyList<Shift>> ListAsync(
        Guid? driverId = null,
        ShiftStatus? status = null,
        CancellationToken cancellationToken = default);

    Task AddAsync(Shift entity);

    void Update(Shift entity);

    /// <summary>
    /// True when the driver already has a shift that overlaps the given
    /// window. Cancelled and aborted shifts do not block new work; pass
    /// <paramref name="excludingShiftId"/> when rescheduling a shift so it
    /// does not collide with itself.
    /// </summary>
    Task<bool> HasOverlappingShift(
        DriverId driverId,
        DateTime start,
        DateTime end,
        ShiftId? excludingShiftId = null);
}

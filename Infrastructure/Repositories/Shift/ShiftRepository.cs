using Domain.Drivers.ValueObjects;
using Domain.Shifts;
using Domain.Shifts.Enums;
using Domain.Shifts.ValueObjects;
using Infrastructure.Persistence.Data;
using Infrastructure.Repositories.Shared;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class ShiftRepository : GenericRepository<Shift>, IShiftRepository
{
    public ShiftRepository(AppDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Tracked lookup for write flows. The generic <see cref="EntityQuery"/>
    /// is AsNoTracking, so handlers that mutate the aggregate must use this
    /// method - otherwise the change is silently discarded on SaveChanges.
    /// </summary>
    public Task<Shift?> GetByIdAsync(ShiftId id, CancellationToken cancellationToken = default)
        => _dbSet.SingleOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Shift>> ListAsync(
        Guid? driverId = null,
        ShiftStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsNoTracking();

        if (driverId is { } driver)
            query = query.Where(s => s.DriverId == new DriverId(driver));

        if (status is { } shiftStatus)
            query = query.Where(s => s.Status == shiftStatus);

        return await query
            .OrderBy(s => s.ShiftStart)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// A driver cannot be booked twice for the same time window. Cancelled
    /// and aborted shifts never occupied the window, so they are excluded;
    /// completed shifts still count (they were actually worked).
    /// </summary>
    public Task<bool> HasOverlappingShift(
        DriverId driverId,
        DateTime start,
        DateTime end,
        ShiftId? excludingShiftId = null)
    {
        var query = _dbSet.AsNoTracking()
            .Where(s => s.DriverId == driverId)
            .Where(s => s.Status != ShiftStatus.Cancelled && s.Status != ShiftStatus.Aborted)
            // Half-open intervals: [start, end) overlaps when each starts before the other ends.
            .Where(s => s.ShiftEnd > start && s.ShiftStart < end);

        if (excludingShiftId is { } excluded)
            query = query.Where(s => s.Id != excluded);

        return query.AnyAsync();
    }
}

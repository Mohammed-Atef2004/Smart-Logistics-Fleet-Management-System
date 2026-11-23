using Domain.Shipments;
using Domain.Shipments.Enums;
using Domain.Shipments.ValueObjects;
using Infrastructure.Persistence.Data;
using Infrastructure.Repositories.Shared;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class ShipmentRepository : GenericRepository<Shipment>, IShipmentRepository
{
    public ShipmentRepository(AppDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Tracked lookup for write flows. The generic <see cref="EntityQuery"/>
    /// is AsNoTracking, so handlers that mutate the aggregate must use this
    /// method. Owned packages and route points are loaded with the owner.
    /// </summary>
    public Task<Shipment?> GetByIdAsync(ShipmentId id, CancellationToken ct = default) =>
        _dbSet
            .AsSplitQuery()
            .SingleOrDefaultAsync(s => s.Id == id, ct);

    public async Task<IReadOnlyList<Shipment>> ListAsync(
        ShipmentStatus? status = null,
        CancellationToken ct = default)
    {
        var query = _dbSet.AsNoTracking();

        if (status is { } value)
            query = query.Where(s => s.Tracking.Status == value);

        return await query
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(ct);
    }

    public Task<Shipment?> GetByTrackingNumberAsync(
        string trackingNumber,
        CancellationToken ct = default) =>
        _dbSet
            .AsSplitQuery()
            .FirstOrDefaultAsync(s => s.Tracking.TrackingNumber == trackingNumber, ct);

    public Task<bool> TrackingNumberExistsAsync(
        string trackingNumber,
        CancellationToken ct = default) =>
        _dbSet.AnyAsync(s => s.Tracking.TrackingNumber == trackingNumber, ct);
}

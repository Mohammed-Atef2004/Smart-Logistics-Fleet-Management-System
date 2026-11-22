using Application.Common.Interfaces;
using Domain.Drivers;
using Domain.Drivers.ValueObjects;
using Infrastructure.Persistence.Data;
using Infrastructure.Repositories.Shared;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class DriverRepository : GenericRepository<Driver>, IDriverRepository
{
    public DriverRepository(AppDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Tracked lookup for write flows. The generic <see cref="EntityQuery"/>
    /// is AsNoTracking, so handlers that mutate the aggregate must use this
    /// method — otherwise the change is silently discarded on SaveChanges.
    /// </summary>
    public Task<Driver?> GetByIdAsync(DriverId id, CancellationToken cancellationToken = default)
        => _dbSet.SingleOrDefaultAsync(d => d.Id == id, cancellationToken);
}

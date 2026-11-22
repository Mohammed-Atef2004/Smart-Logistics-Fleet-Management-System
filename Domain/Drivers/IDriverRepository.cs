using Domain.Drivers.ValueObjects;

namespace Domain.Drivers;

/// <summary>
/// Repository contract for the Driver aggregate.
/// Owned by the Domain (classic DDD); implemented by Infrastructure.
/// </summary>
public interface IDriverRepository
{
    /// <summary>Deferred query root for read-side composition.</summary>
    IQueryable<Driver> EntityQuery { get; }

    Task<Driver?> GetByIdAsync(DriverId id, CancellationToken cancellationToken = default);

    Task AddAsync(Driver entity);

    void Update(Driver entity);
}

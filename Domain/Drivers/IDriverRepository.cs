namespace Domain.Drivers;

/// <summary>
/// Repository contract for the Driver aggregate.
/// Owned by the Domain (classic DDD); implemented by Infrastructure.
/// </summary>
public interface IDriverRepository
{
    /// <summary>Deferred query root for read-side composition.</summary>
    IQueryable<Driver> EntityQuery { get; }

    Task AddAsync(Driver entity);
}

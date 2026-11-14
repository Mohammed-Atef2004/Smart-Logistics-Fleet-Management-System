namespace Domain.Warehouse;

/// <summary>
/// Repository contract for the Warehouse aggregate.
/// Owned by the Domain (classic DDD); implemented by Infrastructure.
/// </summary>
public interface IWarehouseRepository
{
    /// <summary>Deferred query root for read-side composition.</summary>
    IQueryable<Warehouse> EntityQuery { get; }

    Task AddAsync(Warehouse entity);
    void Update(Warehouse entity);
}

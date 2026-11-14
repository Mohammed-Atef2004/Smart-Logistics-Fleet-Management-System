namespace Domain.InventoryItems;

/// <summary>
/// Repository contract for the InventoryItem aggregate.
/// Owned by the Domain (classic DDD); implemented by Infrastructure.
/// </summary>
public interface IInventoryItemRepository
{
    /// <summary>Deferred query root for read-side composition.</summary>
    IQueryable<InventoryItem> EntityQuery { get; }

    Task AddAsync(InventoryItem entity);
    void Update(InventoryItem entity);
}

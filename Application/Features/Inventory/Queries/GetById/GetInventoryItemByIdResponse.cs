using Domain.Inventory.ValueObjects;
using Domain.Warehouse.ValueObjects;

namespace Application.Inventory.GetInventoryItemById
{

    public sealed record GetInventoryItemByIdResponse(
        Guid Id,
        bool IsActive,
        string Sku,
        string ProductName,
        string? Description,
        int ReorderThreshold,
        bool NeedsReorder,
        bool IsOutOfStock,
        decimal WeightValue,
        string WeightUnit,
        WarehouseId? WarehouseId,
        StorageLocationId? StorageLocationId);
}
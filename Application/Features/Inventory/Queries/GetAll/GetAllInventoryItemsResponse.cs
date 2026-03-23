namespace Application.Inventory.GetAllInventoryItems
{

    public sealed record GetAllInventoryItemsResponse(
        Guid Id,
        string Sku,
        string ProductName,
        int Quantity,
        bool NeedsReorder,
        bool IsOutOfStock,
        bool IsActive);
}
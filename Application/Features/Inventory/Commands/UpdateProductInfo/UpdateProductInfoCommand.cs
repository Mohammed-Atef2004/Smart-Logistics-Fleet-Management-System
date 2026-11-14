namespace Application.Inventory.UpdateProductInfo
{

    public sealed record UpdateProductInfoCommand(
        Guid InventoryItemId,
        string Sku,
        string Name,
        string? Description) : IRequest<Result>;
}
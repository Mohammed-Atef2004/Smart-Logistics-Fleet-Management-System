namespace Application.Inventory.DeactivateInventoryItem
{

    public sealed record DeactivateInventoryItemCommand(Guid InventoryItemId) : IRequest<Result>;
}
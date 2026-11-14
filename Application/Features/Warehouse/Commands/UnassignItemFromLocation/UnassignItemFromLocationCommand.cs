namespace Application.Warehouses.UnassignItemFromLocation
{

    public sealed record UnassignItemFromLocationCommand(
        Guid WarehouseId,
        Guid StorageLocationId,
        Guid InventoryItemId) : IRequest<Result>;
}
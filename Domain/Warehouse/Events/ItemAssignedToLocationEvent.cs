using Domain.Inventory.ValueObjects;
using Domain.SharedKernel;
using Domain.Warehouse.ValueObjects;

namespace Domain.Warehouse.Events
{
    public sealed record ItemAssignedToLocationEvent(
        WarehouseId WarehouseId,
        StorageLocationId LocationId,
        InventoryItemId ItemId) : DomainEvent;
}

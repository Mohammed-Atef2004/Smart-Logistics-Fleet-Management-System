using Domain.Inventory.ValueObjects;
using Domain.SharedKernel;
using Domain.Warehouse.ValueObjects;

namespace Domain.Inventory.Events
{
    public sealed record ItemLocationAssignedEvent(
        InventoryItemId ItemId,
        WarehouseId WarehouseId,
        StorageLocationId LocationId) : DomainEvent;
}

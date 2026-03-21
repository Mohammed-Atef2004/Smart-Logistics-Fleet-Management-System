using Domain.SharedKernel;
using Domain.Warehouse.ValueObjects;

namespace Domain.Warehouse.Events
{
    public sealed record StorageLocationRemovedEvent(
        WarehouseId WarehouseId,
        StorageLocationId LocationId) : DomainEvent;
}

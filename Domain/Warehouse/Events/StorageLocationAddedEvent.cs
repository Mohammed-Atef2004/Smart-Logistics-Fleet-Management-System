using Domain.SharedKernel;
using Domain.Warehouse.ValueObjects;

namespace Domain.Warehouse.Events
{
    public sealed record StorageLocationAddedEvent(
        WarehouseId WarehouseId,
        StorageLocationId LocationId,
        string Name) : DomainEvent;
}

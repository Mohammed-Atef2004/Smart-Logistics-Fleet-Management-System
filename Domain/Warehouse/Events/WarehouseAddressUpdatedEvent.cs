using Domain.SharedKernel;
using Domain.Warehouse.ValueObjects;

namespace Domain.Warehouse.Events
{
    public sealed record WarehouseAddressUpdatedEvent(
        WarehouseId WarehouseId,
        Address NewAddress) : DomainEvent;
}

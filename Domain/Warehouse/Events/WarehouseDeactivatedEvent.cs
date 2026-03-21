using Domain.SharedKernel;
using Domain.Warehouse.ValueObjects;

namespace Domain.Warehouse.Events
{
    public sealed record WarehouseDeactivatedEvent(
        WarehouseId WarehouseId) : DomainEvent;
}

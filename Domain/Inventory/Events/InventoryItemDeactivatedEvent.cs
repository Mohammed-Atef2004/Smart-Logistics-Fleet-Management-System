using Domain.Inventory.ValueObjects;
using Domain.SharedKernel;

namespace Domain.Inventory.Events
{
    public sealed record InventoryItemDeactivatedEvent(
        InventoryItemId ItemId) : DomainEvent;
}

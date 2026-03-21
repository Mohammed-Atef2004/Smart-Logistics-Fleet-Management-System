using Domain.Inventory.ValueObjects;
using Domain.SharedKernel;

namespace Domain.Inventory.Events
{
    public sealed record StockRemovedEvent(
        InventoryItemId ItemId,
        int UnitsRemoved,
        int NewQuantity) : DomainEvent;
}

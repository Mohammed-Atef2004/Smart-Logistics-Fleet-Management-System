using Domain.Inventory.ValueObjects;
using Domain.SharedKernel;

namespace Domain.Inventory.Events
{
    public sealed record StockAddedEvent(
        InventoryItemId ItemId,
        int UnitsAdded,
        int NewQuantity) : DomainEvent;
}

using Domain.Inventory.ValueObjects;
using Domain.SharedKernel;

namespace Domain.Inventory.Events
{
    public sealed record ReorderNeededEvent(
        InventoryItemId ItemId,
        string Sku,
        int CurrentQuantity) : DomainEvent;
}

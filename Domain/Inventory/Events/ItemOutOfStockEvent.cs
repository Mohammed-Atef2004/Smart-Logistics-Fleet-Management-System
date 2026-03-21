using Domain.Inventory.ValueObjects;
using Domain.SharedKernel;

namespace Domain.Inventory.Events
{
    public sealed record ItemOutOfStockEvent(
        InventoryItemId ItemId,
        string Sku) : DomainEvent;
}

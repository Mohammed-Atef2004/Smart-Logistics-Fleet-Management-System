using Domain.Common;
using Domain.Inventory.ValueObjects;
using Domain.InventoryItems;
using Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Inventory.Events
{
    public sealed record InventoryItemCreatedEvent(
    InventoryItemId ItemId,
    string Sku,
    int InitialQuantity) : DomainEvent;
}

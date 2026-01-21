using Domain.Common;
using Domain.Warehouse.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Warehouse.Events
{

    public class InventoryMovedEvent : DomainEvent
    {
        public Guid InventoryItemId { get; }
        public StorageLocation OldLocation { get; }
        public StorageLocation NewLocation { get; }

        public InventoryMovedEvent(
            Guid inventoryItemId,
            StorageLocation oldLocation,
            StorageLocation newLocation)
        {
            InventoryItemId = inventoryItemId;
            OldLocation = oldLocation;
            NewLocation = newLocation;
        }
    }
}

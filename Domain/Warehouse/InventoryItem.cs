using Domain.Common;
using Domain.ValueObjects;
using Domain.Warehouse.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Warehouse
{
    public class InventoryItem : AggregateRoot
    {
        public string Name { get; private set; }
        public int Quantity { get; private set; }
        public StorageLocation Location { get; private set; }

        private InventoryItem() { } // EF

        public InventoryItem(string name, int quantity, StorageLocation location)
        {
            Name = name;
            Quantity = quantity;
            Location = location;
        }

        public void Move(StorageLocation newLocation)
        {
            var oldLocation = Location;
            Location = newLocation;

            AddDomainEvent(
                new InventoryMovedEvent(Id, oldLocation, newLocation)
            );
        }

        public void IncreaseQuantity(int amount)
        {
            Quantity += amount;
        }

        public void DecreaseQuantity(int amount)
        {
            if (Quantity - amount < 0)
                throw new DomainException("Insufficient inventory.");

            Quantity -= amount;
        }
    }
}

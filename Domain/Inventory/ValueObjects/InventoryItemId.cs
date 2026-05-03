using Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Inventory.ValueObjects
{
    public sealed class InventoryItemId
    {
        public Guid Value { get; private set; }   

        private InventoryItemId() { } 

        public InventoryItemId(Guid value)
        {
            Value = value;
        }
        public static InventoryItemId New()
        {
            return new InventoryItemId(Guid.NewGuid());
        }
        public static InventoryItemId From(Guid value)
        {
            return new InventoryItemId(value);
        }
    }
}

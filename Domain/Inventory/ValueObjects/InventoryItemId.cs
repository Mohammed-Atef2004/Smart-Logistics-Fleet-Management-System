using Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Inventory.ValueObjects
{
    public record InventoryItemId(Guid Id)
    {
        public static InventoryItemId New(Guid Id) => new InventoryItemId(Id);
        public static InventoryItemId From(Guid Id) => new InventoryItemId(Id);
        public static InventoryItemId From(string Id) => new InventoryItemId(Guid.Parse(Id));
       
    }
}

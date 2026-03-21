using Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Inventory.Errors
{

    public static class InventoryErrors
    {
        public static Error Inactive = new("InventoryItem.Inactive", "Inventory item is not active.");
        public static Error AlreadyInactive = new("InventoryItem.AlreadyInactive", "Inventory item is already inactive.");
        public static Error AlreadyAssignedToLocation = new("InventoryItem.AlreadyAssignedToLocation", "Inventory item is already assigned to a storage location.");
        public static Error NotAssignedToLocation = new("InventoryItem.NotAssignedToLocation", "Inventory item is not assigned to any storage location.");
        public static Error CannotDeactivateAssignedItem = new("InventoryItem.CannotDeactivateAssignedItem", "Cannot deactivate an item that is assigned to a storage location.");
        public static Error NotFound = new("InventoryItem.NotFound", "Inventory item was not found.");

       

        
      
    }
    
}

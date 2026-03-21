using Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Warehouse.Errors
{
    public static class WarehouseErrors
    {
        public static Error EmptyName 
            = new("Warehouse.EmptyName", "Warehouse name cannot be empty.");
        public static Error Inactive
            = new("Warehouse.Inactive", "Warehouse is not active.");
        public static Error AlreadyInactive 
            = new("Warehouse.AlreadyInactive", "Warehouse is already inactive.");
        public static Error HasAssignedItems
            = new("Warehouse.HasAssignedItems", "Cannot deactivate warehouse while storage locations have assigned items.");
        public static Error NotFound 
            = new("Warehouse.NotFound", "Warehouse was not found.");
    
    }
}


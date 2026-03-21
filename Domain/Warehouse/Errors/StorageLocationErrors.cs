using Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Warehouse.Errors
{
    public class StorageLocationErrors
    {
        public static Error LocationEmptyName = new("StorageLocation.EmptyName", "Storage location name cannot be empty.");
        public static Error LocationNotFound = new("StorageLocation.NotFound", "Storage location was not found.");
        public static Error LocationAlreadyExists = new("StorageLocation.AlreadyExists", "Storage location already exists.");
        public static Error LocationInactive = new("StorageLocation.Inactive", "Storage location is not active.");
        public static Error LocationAlreadyInactive = new("StorageLocation.AlreadyInactive", "Storage location is already inactive.");
        public static Error LocationHasItems = new("StorageLocation.HasAssignedItems", "Cannot remove storage location while it has assigned items.");
        public static Error ItemAlreadyAssigned = new("StorageLocation.ItemAlreadyAssigned", "Item is already assigned to this location.");
        public static Error ItemNotAssigned = new("StorageLocation.ItemNotAssigned", "Item is not assigned to this location.");
    }
}

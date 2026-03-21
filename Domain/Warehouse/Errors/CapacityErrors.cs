using Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Warehouse.Errors
{
    public class CapacityErrors
    {

        public static Error InvalidMaxSlots = new("Capacity.InvalidMaxSlots", "Max slots must be greater than zero.");
        public static Error NegativeUsedSlots = new("Capacity.NegativeUsedSlots", "Used slots cannot be negative.");
        public static Error UsedExceedsMax = new("Capacity.UsedExceedsMax", "Used slots cannot exceed max slots.");
        public static Error InsufficientCapacity = new("Capacity.InsufficientCapacity", "Insufficient capacity in storage location.");
        public static Error InvalidSlotsCount = new("Capacity.InvalidSlotsCount", "Slots count must be greater than zero.");
        public static Error ReleaseBelowZero = new("Capacity.ReleaseBelowZero", "Cannot release more slots than currently used.");
    }
}

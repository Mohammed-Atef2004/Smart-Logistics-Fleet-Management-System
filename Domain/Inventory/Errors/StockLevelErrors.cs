using Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Inventory.Errors
{
    public class StockLevelErrors
    {
        public static Error NegativeQuantity = new("StockLevel.NegativeQuantity", "Quantity cannot be negative.");
        public static Error NegativeReorderThreshold = new("StockLevel.NegativeReorderThreshold", "Reorder threshold cannot be negative.");
        public static Error InsufficientStock = new("StockLevel.InsufficientStock", "Insufficient stock to complete the operation.");
        public static Error InvalidUnits = new("StockLevel.InvalidUnits", "Units must be greater than zero.");
    }
}

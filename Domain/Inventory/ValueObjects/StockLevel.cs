using Domain.Common.Domain.Common;
using Domain.Inventory.Errors;
using Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Inventory.ValueObjects
{
    public sealed class StockLevel : ValueObject
    {
        public int Quantity { get; }
        public int ReorderThreshold { get; }
        public bool NeedsReorder => Quantity <= ReorderThreshold;
        public bool IsOutOfStock => Quantity == 0;

        private StockLevel() { }

        private StockLevel(int quantity, int reorderThreshold)
        {
            Quantity = quantity;
            ReorderThreshold = reorderThreshold;
        }

        public static Result<StockLevel> Create(int quantity, int reorderThreshold)
        {
            if (quantity < 0)
                return Result<StockLevel>.Failure(StockLevelErrors.NegativeQuantity);

            if (reorderThreshold < 0)
                return Result<StockLevel>.Failure(StockLevelErrors.NegativeReorderThreshold);

            return Result<StockLevel>.Success(new StockLevel(quantity, reorderThreshold));
        }

        public Result<StockLevel> Add(int units)
        {
            if (units <= 0)
                return Result<StockLevel>.Failure(StockLevelErrors.InvalidUnits);

            return Result<StockLevel>.Success(new StockLevel(Quantity + units, ReorderThreshold));
        }

        public Result<StockLevel> Remove(int units)
        {
            if (units <= 0)
                return Result<StockLevel>.Failure(StockLevelErrors.InvalidUnits);

            if (Quantity - units < 0)
                return Result<StockLevel>.Failure(StockLevelErrors.InsufficientStock);

            return Result<StockLevel>.Success(new StockLevel(Quantity - units, ReorderThreshold));
        }

        public Result<StockLevel> AdjustReorderThreshold(int newThreshold)
        {
            if (newThreshold < 0)
                return Result<StockLevel>.Failure(StockLevelErrors.NegativeReorderThreshold);

            return Result<StockLevel>.Success(new StockLevel(Quantity, newThreshold));
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Quantity;
            yield return ReorderThreshold;
        }
    }
}

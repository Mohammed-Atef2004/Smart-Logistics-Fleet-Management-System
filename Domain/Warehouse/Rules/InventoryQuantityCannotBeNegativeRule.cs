using Domain.Common;

namespace Domain.Warehouse.Rules
{
    public class InventoryQuantityCannotBeNegativeRule : IBusinessRule
    {
        private readonly int _quantity;

        public InventoryQuantityCannotBeNegativeRule(int quantity)
        {
            _quantity = quantity;
        }

        public bool IsBroken() => _quantity < 0;

        public string Message => "Quantity cannot be negative.";
    }
}

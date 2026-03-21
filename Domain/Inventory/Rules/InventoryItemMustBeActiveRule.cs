using Domain.Inventory.Errors;
using Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Inventory.Rules
{
    public sealed class InventoryItemMustBeActiveRule : IBusinessRule
    {
        private readonly bool _isActive;

        public InventoryItemMustBeActiveRule(bool isActive)
            => _isActive = isActive;

        public bool IsBroken() => !_isActive;

        public Error Error => InventoryErrors.Inactive;
    }
}

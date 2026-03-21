using Domain.SharedKernel;
using Domain.Warehouse.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Warehouse.Rules
{
    public sealed class WarehouseMustBeActiveRule : IBusinessRule
    {
        private readonly bool _isActive;

        public WarehouseMustBeActiveRule(bool isActive)
            => _isActive = isActive;

        public bool IsBroken() => !_isActive;

        public Error Error => WarehouseErrors.Inactive;
    }
}

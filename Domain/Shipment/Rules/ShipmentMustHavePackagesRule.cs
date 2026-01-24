using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Shipment.Rules
{
    public class ShipmentMustHavePackagesRule : IBusinessRule
    {
        private readonly int _packageCount;

        public ShipmentMustHavePackagesRule(int packageCount)
        {
            _packageCount = packageCount;
        }

        public bool IsBroken() => _packageCount == 0;

        public string Message => "Shipment must contain at least one package";
    }
}

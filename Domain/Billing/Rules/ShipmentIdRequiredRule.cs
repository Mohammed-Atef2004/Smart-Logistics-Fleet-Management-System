using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Billing.Rules
{
    public class ShipmentIdRequiredRule : IBusinessRule
    {
        private readonly Guid _shipmentId;

        public ShipmentIdRequiredRule(Guid shipmentId)
        {
            _shipmentId = shipmentId;
        }

        public bool IsBroken() => _shipmentId == Guid.Empty;

        public string Message => "ShipmentId is required.";
    }
}

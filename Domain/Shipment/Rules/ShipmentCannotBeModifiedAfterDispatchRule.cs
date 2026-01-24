using Domain.Common;
using Domain.Shipment.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Shipment.Rules
{
    public class ShipmentCannotBeModifiedAfterDispatchRule : IBusinessRule
    {
        private readonly ShipmentStatus _status;

        public ShipmentCannotBeModifiedAfterDispatchRule(ShipmentStatus status)
        {
            _status = status;
        }

        public bool IsBroken() => _status != ShipmentStatus.Created;

        public string Message => "Cannot modify shipment after it has been dispatched";
    }

}

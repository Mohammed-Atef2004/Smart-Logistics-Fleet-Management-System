using Domain.Common;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Shipment.Rules
{
    public class ShipmentCannotBeDeliveredTwiceRule : IBusinessRule
    {
        private readonly ShipmentStatus _status;

        public ShipmentCannotBeDeliveredTwiceRule(ShipmentStatus status)
        {
            _status = status;
        }

        public bool IsBroken() => _status == ShipmentStatus.Delivered;

        public string Message => "Shipment is already delivered.";
    }

}

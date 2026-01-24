using Domain.Common;
using Domain.Shipment.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Shipment.Rules
{
    public class ShipmentMustBeInTransitForDeliveryRule : IBusinessRule
    {
        private readonly ShipmentStatus _status;

        public ShipmentMustBeInTransitForDeliveryRule(ShipmentStatus status)
        {
            _status = status;
        }

        public bool IsBroken() => _status != ShipmentStatus.InTransit && _status != ShipmentStatus.OutForDelivery;

        public string Message => "Shipment must be in transit or out for delivery to be delivered";
    }
}

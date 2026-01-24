using Domain.Common;
using Domain.Shipment.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Shipment.Rules
{
    public class CannotAddTrackingToFinalizedShipmentRule : IBusinessRule
    {
        private readonly ShipmentStatus _status;

        public CannotAddTrackingToFinalizedShipmentRule(ShipmentStatus status)
        {
            _status = status;
        }

        public bool IsBroken() => _status == ShipmentStatus.Delivered ||
                                   _status == ShipmentStatus.Cancelled ||
                                   _status == ShipmentStatus.Returned;

        public string Message => "Cannot add tracking updates to finalized shipment";
    }
}

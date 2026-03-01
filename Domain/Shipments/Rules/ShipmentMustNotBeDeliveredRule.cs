using Domain.SharedKernel;
using Domain.Shipments.Enums;

namespace Domain.Shipments.Rules
{
    internal class ShipmentMustNotBeDeliveredRule : IBusinessRule
    {
        private readonly ShipmentStatus _status;
        public ShipmentMustNotBeDeliveredRule(ShipmentStatus status)
        {
            _status = status;
        }
        public Error Error => new("Shipment.MustNotBeDelivered", "Shipment has already been delivered");

        public bool IsBroken()
        {
            return _status == ShipmentStatus.Delivered;
        }
    }
}
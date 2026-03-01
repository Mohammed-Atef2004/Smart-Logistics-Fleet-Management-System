using Domain.SharedKernel;
using Domain.Shipments.Enums;

namespace Domain.Shipments.Rules
{
    internal class ShipmentMustNotBeCancelledRule : IBusinessRule
    {
        private ShipmentStatus status;

        public ShipmentMustNotBeCancelledRule(ShipmentStatus status)
        {
            this.status = status;
        }

        public Error Error=>new Error("Shipment.CannotBeCancelled", "Shipment cannot be cancelled in its current status.");

        public bool IsBroken()
        {
            return status == ShipmentStatus.Cancelled;
        }
    }
}
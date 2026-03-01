using Domain.SharedKernel;
using Domain.Shipments.Enums;

namespace Domain.Shipments.Rules
{
    public class ShipmentMustBeEditableRule : IBusinessRule
    {
        private readonly ShipmentStatus status;

        public ShipmentMustBeEditableRule(ShipmentStatus status)
        {
            this.status = status;
        }

        public Error Error => new("Shipment.MustBeEditable", "Shipment must be in a state that allows editing.");

        public bool IsBroken()
        {
          return status == ShipmentStatus.Delivered || status == ShipmentStatus.Cancelled;
        }
    }
}
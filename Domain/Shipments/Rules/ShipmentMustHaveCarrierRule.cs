using Domain.SharedKernel;

namespace Domain.Shipments.Rules
{
    internal class ShipmentMustHaveCarrierRule : IBusinessRule
    {
        private readonly string _carrierName;
        public ShipmentMustHaveCarrierRule(string carrierName)
        {
            _carrierName = carrierName;
        }
        public Error Error => new Error("Shipment.MustHaveCarrier", "Shipment must have a carrier assigned");

        public bool IsBroken()
        {
            return string.IsNullOrWhiteSpace(_carrierName);
        }
    }
}
using Domain.SharedKernel;
using Domain.Shipments.ValueObjects;
using System.Runtime.CompilerServices;

namespace Domain.Shipments.Rules
{
    public class ShipmentMustNotBeTerminalRule : IBusinessRule
    {
        private readonly TrackingInfo _trackingInfo;
        public ShipmentMustNotBeTerminalRule(TrackingInfo trackingInfo)
        {
            _trackingInfo = trackingInfo;
        }
        public Error Error => new("Shipment.MustNotBeTerminal", "Shipment is already in terminal state");

        public bool IsBroken()
        {
            return _trackingInfo.IsTerminal;
        }
    }
}
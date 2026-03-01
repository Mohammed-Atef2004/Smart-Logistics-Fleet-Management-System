using Domain.SharedKernel;

namespace Domain.Shipments.Rules
{
    public class ShipmentMustHavePackagesRule : IBusinessRule
    {
        private readonly int _count;

        public ShipmentMustHavePackagesRule(int count)
        {
            _count = count;
        }

        public bool IsBroken()
        {
            return _count < 1;
        }
        public  Error Error => new("ShipmentMustHavePackages", "A shipment must contain at least one package.");
    }
}
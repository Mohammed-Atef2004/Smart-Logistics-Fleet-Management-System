using Domain.SharedKernel;

namespace Domain.Shipments.Rules
{
    internal class DeclaredValueMustNotBeNegativeRule: IBusinessRule
    {
        private decimal declaredValue;

        public DeclaredValueMustNotBeNegativeRule(decimal declaredValue)
        {
            this.declaredValue = declaredValue;
        }

        public Error Error => new Error("DeclaredValueMustNotBeNegative", "Declared value cannot be negative.");

        public bool IsBroken()
        {
            return declaredValue < 0;
        }
    }
}
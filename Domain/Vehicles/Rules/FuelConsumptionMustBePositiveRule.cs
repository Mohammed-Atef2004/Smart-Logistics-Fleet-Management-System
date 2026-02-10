using Domain.Common;
using Domain.Vehicles;
using Domain.Vehicles.Errors;

namespace Domain.Vehicles.Rules
{
    public class FuelConsumptionMustBePositiveRule : IBusinessRule
    {
        private readonly FuelConsumption _consumption;

        public FuelConsumptionMustBePositiveRule(FuelConsumption consumption)
        {
            _consumption = consumption;
        }

        public bool IsBroken() => _consumption.LitersPer100Km <= 0;

        public Error Error => VehicleErrors.InvalidFuelConsumption;
    }
}

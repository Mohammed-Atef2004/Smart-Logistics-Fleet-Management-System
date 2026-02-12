using Domain.SharedKernel;
using Domain.Vehicles;
using Domain.Vehicles.Errors;
using Domain.Vehicles.ValueObjects;

namespace Domain.Vehicles.Rules
{
    public class FuelConsumptionMustBePositiveRule : IBusinessRule
    {
        private readonly FuelConsumption _consumption;

        public FuelConsumptionMustBePositiveRule(FuelConsumption consumption)
        {
            _consumption = consumption;
        }

        public bool IsBroken() => _consumption.Liters <= 0;

        public Error Error => VehicleErrors.InvalidFuelConsumption;
    }
}

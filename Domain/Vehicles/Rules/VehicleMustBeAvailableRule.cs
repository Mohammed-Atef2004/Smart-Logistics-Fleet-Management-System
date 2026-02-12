using Domain.SharedKernel;
using Domain.Vehicles.Enums;
using Domain.Vehicles.Errors;

namespace Domain.Vehicles.Rules
{
    public class VehicleMustBeAvailableRule : IBusinessRule
    {
        private readonly VehicleStatus _status;

        public VehicleMustBeAvailableRule(VehicleStatus status)
        {
            _status = status;
        }

        public bool IsBroken() => _status != VehicleStatus.Available;

        public Error Error => VehicleErrors.NotAvailable;
    }
}

using Domain.SharedKernel;
using Domain.Vehicles.Enums;
using Domain.Vehicles.Errors;

namespace Domain.Vehicles.Rules
{
    public class VehicleCannotBeRetiredRule : IBusinessRule
    {
        private readonly VehicleStatus _status;

        public VehicleCannotBeRetiredRule(VehicleStatus status)
        {
            _status = status;
        }

        public bool IsBroken() => _status == VehicleStatus.Retired;

        public Error Error => VehicleErrors.VehicleAlreadyRetired;
    }
}

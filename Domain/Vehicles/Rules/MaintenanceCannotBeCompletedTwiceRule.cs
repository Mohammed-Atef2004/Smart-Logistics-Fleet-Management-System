using Domain.Common;
using Domain.Vehicles.Errors;

namespace Domain.Vehicles.Rules
{
    public class MaintenanceCannotBeCompletedTwiceRule : IBusinessRule
    {
        private readonly bool _isCompleted;

        public MaintenanceCannotBeCompletedTwiceRule(bool isCompleted)
        {
            _isCompleted = isCompleted;
        }

        public bool IsBroken() => _isCompleted;

        public Error Error => MaintenanceErrors.AlreadyCompleted;
    }
}

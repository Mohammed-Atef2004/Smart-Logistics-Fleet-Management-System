using Domain.SharedKernel;
using Domain.Vehicles.Errors;

namespace Domain.Vehicles.Rules
{
    public class MaintenanceCannotRescheduleCompletedRule : IBusinessRule
    {
        private readonly bool _isCompleted;

        public MaintenanceCannotRescheduleCompletedRule(bool isCompleted)
        {
            _isCompleted = isCompleted;
        }

        public bool IsBroken() => _isCompleted;

        public Error Error => MaintenanceErrors.CannotRescheduleCompleted;
    }
}

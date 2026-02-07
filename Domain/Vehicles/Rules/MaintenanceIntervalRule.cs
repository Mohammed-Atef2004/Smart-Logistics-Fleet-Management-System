using Domain.Common;

namespace Domain.Fleet.Rules
{
    public class MaintenanceIntervalRule : IBusinessRule
    {
        private readonly int _currentMileage;
        private readonly int _lastMaintenanceMileage;

        public MaintenanceIntervalRule(int currentMileage, int lastMaintenanceMileage)
        {
            _currentMileage = currentMileage;
            _lastMaintenanceMileage = lastMaintenanceMileage;
        }

        public string Message => "Vehicle requires maintenance: mileage interval exceeded 10,000 km.";

        // The rule is broken if the difference is 10k or more
        public bool IsBroken() => (_currentMileage - _lastMaintenanceMileage) >= 10000;
    }
}
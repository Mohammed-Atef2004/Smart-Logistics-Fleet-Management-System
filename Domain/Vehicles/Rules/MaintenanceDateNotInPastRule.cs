using Domain.SharedKernel;
using Domain.Vehicles.Errors;
using System;

namespace Domain.Vehicles.Rules
{
    public class MaintenanceDateNotInPastRule : IBusinessRule
    {
        private readonly DateTime _date;

        public MaintenanceDateNotInPastRule(DateTime date)
        {
            _date = date;
        }

        public bool IsBroken() => _date.Date < DateTime.UtcNow.Date;

        public Error Error => MaintenanceErrors.DateInPast;
    }
}

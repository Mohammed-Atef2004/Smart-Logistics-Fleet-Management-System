using Domain.Drivers.Enums;
using Domain.SharedKernel;

namespace Domain.Drivers.Rules
{
    internal class DriverMustBeSuspendedRule : IBusinessRule
    {
        private readonly DriverStatus Status;

        public DriverMustBeSuspendedRule(DriverStatus status)
        {
            Status = status;
        }

        public Error Error =>new Error("DriverSuspendedError","Driver Must Be Suspended");

        public bool IsBroken()
        {
            return Status != DriverStatus.Suspended;    
        }
    }
}
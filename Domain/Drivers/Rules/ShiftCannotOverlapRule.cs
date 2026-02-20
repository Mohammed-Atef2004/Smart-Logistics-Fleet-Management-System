//using Domain.Drivers.Errors;
//using Domain.SharedKernel;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Domain.Drivers.Rules
//{
//    public class ShiftCannotOverlapRule : IBusinessRule
//    {
//        private readonly IReadOnlyCollection<DriverShift> _shifts;
//        public ShiftCannotOverlapRule(IReadOnlyCollection<DriverShift> shifts) => _shifts = shifts;

//        public bool IsBroken() => _shifts.Any(s => s.IsActive());
//        public Error Error => DriverErrors.ShiftAlreadyActive;
//    }
//}

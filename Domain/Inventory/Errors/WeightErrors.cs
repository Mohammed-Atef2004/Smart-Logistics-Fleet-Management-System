using Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Inventory.Errors
{
    public class WeightErrors
    {
        public static Error InvalidWeightValue = new("Weight.InvalidValue", "Weight value must be greater than zero.");
        public static Error InvalidWeightUnit = new("Weight.InvalidUnit", "Weight unit is not valid.");
    }
}

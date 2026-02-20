using Domain.Drivers.Errors;
using Domain.SharedKernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Drivers.Rules
{
    public class RatingMustBeValidRule : IBusinessRule
    {
        private readonly double _rating;
        public RatingMustBeValidRule(double rating) => _rating = rating;

        public bool IsBroken() => _rating < 0 || _rating > 5;
        public Error Error => DriverErrors.InvalidRating;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Drivers.ValueObjects
{

    public sealed class DriverRating
    {
        public double Value { get; private set; }
        public int Count { get; private set; }

        private DriverRating() { } // EF Core

        private DriverRating(double value, int count)
        {
            Value = value;
            Count = count;
        }

        public static DriverRating CreateNew() => new(5.0, 0);

        public DriverRating AddRating(double newRating)
        {
            var newCount = Count + 1;
            var newValue = ((Value * Count) + newRating) / newCount;

            return new DriverRating(newValue, newCount);
        }
    }
}

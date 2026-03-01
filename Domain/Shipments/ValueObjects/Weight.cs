using Domain.Shipments.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Shipments.ValueObjects
{
    public sealed record Weight
    {
        public decimal Value { get; init; }
        public WeightUnit Unit { get; init; }

        private Weight(decimal value, WeightUnit unit)
        {
            if (value < 0) throw new ArgumentException("Weight value cannot be negative.", nameof(value));
            Value = value;
            Unit = unit;
        }

        public static Weight InKilograms(decimal value) => new(value, WeightUnit.Kg);
        public static Weight InGrams(decimal value) => new(value, WeightUnit.Gram);
        public static Weight InPounds(decimal value) => new(value, WeightUnit.Pound);

        public decimal ToKilograms() => Unit switch
        {
            WeightUnit.Kg => Value,
            WeightUnit.Gram => Value / 1000m,
            WeightUnit.Pound => Value * 0.453592m,
            _ => throw new InvalidOperationException("Unknown weight unit")
        };

        public override string ToString() => $"{Value} {Unit}";
    }

}

using Domain.Shipments.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Shipments.ValueObjects
{
    public sealed record Dimensions
    {
        public decimal Length { get; init; }
        public decimal Width { get; init; }
        public decimal Height { get; init; }
        public DimensionUnit Unit { get; init; }

        private Dimensions(decimal length, decimal width, decimal height, DimensionUnit unit)
        {
            if (length <= 0 || width <= 0 || height <= 0)
                throw new ArgumentException("All dimensions must be positive values.");
            Length = length;
            Width = width;
            Height = height;
            Unit = unit;
        }

        public static Dimensions InCentimeters(decimal length, decimal width, decimal height) =>
            new(length, width, height, DimensionUnit.Cm);

        public static Dimensions InInches(decimal length, decimal width, decimal height) =>
            new(length, width, height, DimensionUnit.Inch);

        /// <summary>Volumetric weight in KG using industry standard 5000 divisor.</summary>
        public decimal VolumetricWeightKg => Unit switch
        {
            DimensionUnit.Cm => Length * Width * Height / 5000m,
            DimensionUnit.Inch => Length * Width * Height * 16.387m / 5000m,
            _ => throw new InvalidOperationException()
        };

        public override string ToString() => $"{Length}x{Width}x{Height} {Unit}";
    }
}

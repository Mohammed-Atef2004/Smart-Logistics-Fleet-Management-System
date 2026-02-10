using Domain.Common;
using Domain.Vehicles.Errors;
using System;

namespace Domain.Vehicles.ValueObjects
{
    public record MaintenanceDescription
    {
        public string Value { get; }

        public MaintenanceDescription(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException(MaintenanceErrors.DescriptionRequired.Message);

            if (value.Length > 200)
                throw new ArgumentException("Description too long");

            Value = value;
        }

        public static implicit operator string(MaintenanceDescription d) => d.Value;
        public static explicit operator MaintenanceDescription(string s) => new MaintenanceDescription(s);
    }

}

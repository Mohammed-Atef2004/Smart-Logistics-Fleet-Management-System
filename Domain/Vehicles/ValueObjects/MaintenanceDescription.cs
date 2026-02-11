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
        // 2. Factory Method returning Result
        public static Result<MaintenanceDescription> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Result<MaintenanceDescription>.Failure(MaintenanceErrors.DescriptionRequired);

            if (value.Length > 200)
                return Result<MaintenanceDescription>.Failure(new Error("MaintenanceDescription.TooLong", "Description cannot exceed 200 characters."));

            return Result<MaintenanceDescription>.Success(new MaintenanceDescription(value));
        }

        public static implicit operator string(MaintenanceDescription d) => d.Value;
        public static explicit operator MaintenanceDescription(string s) => new MaintenanceDescription(s);
    }

}

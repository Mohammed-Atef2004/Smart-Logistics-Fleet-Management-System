using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Vehicles.ValueObjects
{
    public record MaintenanceRemarks
    {
        public string? Value { get; init; }

        public MaintenanceRemarks(string? value)
        {
            if (value != null && value.Length > 500)
                throw new ArgumentException("Remarks too long");

            Value = value;
        }

        public static implicit operator string?(MaintenanceRemarks r) => r.Value;
        public static explicit operator MaintenanceRemarks(string? s) => new MaintenanceRemarks(s);
    }
}

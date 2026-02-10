using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Vehicles.Errors
{
    public static class MaintenanceErrors
    {
        public static Error DescriptionRequired =>
            new("Maintenance.DescriptionRequired", "Description is required");

        public static Error DateInPast =>
            new("Maintenance.DateInPast", "Maintenance date cannot be in the past");

        public static Error AlreadyCompleted =>
            new("Maintenance.AlreadyCompleted", "Maintenance is already completed");

        public static Error CannotRescheduleCompleted =>
            new("Maintenance.CannotRescheduleCompleted", "Cannot reschedule completed maintenance");
    }

}

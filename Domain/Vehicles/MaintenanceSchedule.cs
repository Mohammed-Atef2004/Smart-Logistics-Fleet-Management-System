using Common.Domain;
using Domain.Common;
using Domain.Vehicles.Errors;
using Domain.Vehicles.ValueObjects;
using System;

namespace Domain.Vehicles
{
    public class MaintenanceSchedule : Entity<Guid>
    {
        public MaintenanceDescription Description { get; private set; }
        public DateTime ScheduledDate { get; private set; }
        public bool IsCompleted { get; private set; }
        public DateTime? CompletedAt { get; private set; }
        public MaintenanceRemarks? Remarks { get; private set; }

        private MaintenanceSchedule() { } // EF Core

        private MaintenanceSchedule(MaintenanceDescription description, DateTime scheduledDate)
        {
            Id = Guid.NewGuid();
            Description = description;
            ScheduledDate = scheduledDate;
            IsCompleted = false;
        }

        // Factory Method
        internal static Result<MaintenanceSchedule> Create(MaintenanceDescription description, DateTime scheduledDate)
        {
            if (scheduledDate.Date < DateTime.UtcNow.Date)
                return Result<MaintenanceSchedule>.Failure(MaintenanceErrors.DateInPast);

            return Result<MaintenanceSchedule>.Success(new MaintenanceSchedule(description, scheduledDate));
        }

        internal Result MarkAsCompleted(MaintenanceRemarks remarks, DateTime completedAt)
        {
            if (IsCompleted)
                return Result.Failure(MaintenanceErrors.AlreadyCompleted);

            IsCompleted = true;
            Remarks = remarks;
            CompletedAt = completedAt;

            return Result.Success();
        }

        internal Result Reschedule(DateTime newDate)
        {
            if (IsCompleted)
                return Result.Failure(MaintenanceErrors.CannotRescheduleCompleted);

            if (newDate.Date < DateTime.UtcNow.Date)
                return Result.Failure(MaintenanceErrors.DateInPast);

            ScheduledDate = newDate;
            return Result.Success();
        }
    }
}

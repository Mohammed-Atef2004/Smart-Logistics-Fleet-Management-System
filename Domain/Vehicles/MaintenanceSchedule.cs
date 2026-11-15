using Domain.SharedKernel;
using Domain.Vehicles.Errors;
using Domain.Vehicles.Rules;
using Domain.Vehicles.ValueObjects;

namespace Domain.Vehicles;

public class MaintenanceSchedule : Entity<Guid>
{
    public MaintenanceDescription Description { get; private set; } = default!;
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

    // Factory Method — now goes through MaintenanceDateNotInPastRule
    // instead of a duplicate inline check (the rule existed unused).
    internal static Result<MaintenanceSchedule> Create(MaintenanceDescription description, DateTime scheduledDate)
    {
        var dateRule = new MaintenanceDateNotInPastRule(scheduledDate);
        if (dateRule.IsBroken())
            return Result<MaintenanceSchedule>.Failure(dateRule.Error);

        return Result<MaintenanceSchedule>.Success(new MaintenanceSchedule(description, scheduledDate));
    }

    internal Result MarkAsCompleted(MaintenanceRemarks? remarks, DateTime completedAt)
    {
        var twiceRule = new MaintenanceCannotBeCompletedTwiceRule(IsCompleted);
        if (twiceRule.IsBroken())
            return Result.Failure(twiceRule.Error);

        IsCompleted = true;
        Remarks = remarks;
        CompletedAt = completedAt;

        return Result.Success();
    }

    internal Result Reschedule(DateTime newDate)
    {
        var completedRule = new MaintenanceCannotRescheduleCompletedRule(IsCompleted);
        if (completedRule.IsBroken())
            return Result.Failure(completedRule.Error);

        var dateRule = new MaintenanceDateNotInPastRule(newDate);
        if (dateRule.IsBroken())
            return Result.Failure(dateRule.Error);

        ScheduledDate = newDate;
        return Result.Success();
    }
}

using Domain.SharedKernel;
using Domain.Vehicles.Enums;
using Domain.Vehicles.Errors;
using Domain.Vehicles.Events;
using Domain.Vehicles.Rules;
using Domain.Vehicles.ValueObjects;

namespace Domain.Vehicles;

public class Vehicle : AggregateRoot<VehicleId>, IAuditable, ISoftDeletable
{
    // ------------------------
    // Private fields / state
    // ------------------------
    private readonly List<MaintenanceSchedule> _maintenanceSchedules = new();

    /// <summary>
    /// Legal status transitions of the Vehicle state machine. Retired is
    /// terminal; anything not listed here (including a no-op transition)
    /// is rejected by <see cref="UpdateStatus"/>.
    /// </summary>
    private static readonly IReadOnlyDictionary<VehicleStatus, VehicleStatus[]> AllowedTransitions =
        new Dictionary<VehicleStatus, VehicleStatus[]>
        {
            [VehicleStatus.Available] = [VehicleStatus.InUse, VehicleStatus.InMaintenance, VehicleStatus.Retired],
            [VehicleStatus.InUse] = [VehicleStatus.Available, VehicleStatus.InMaintenance, VehicleStatus.Retired],
            [VehicleStatus.InMaintenance] = [VehicleStatus.Available, VehicleStatus.Retired],
            // Retired is terminal — a retired vehicle never returns to service.
            [VehicleStatus.Retired] = [],
        };

    // ------------------------
    // Properties
    // ------------------------
    public VehiclePlateNumber PlateNumber { get; private set; } = default!;
    public VehicleSpecification Specification { get; private set; } = default!;
    public FuelConsumption? FuelConsumption { get; private set; }
    public VehicleStatus Status { get; private set; }

    // Reference to another aggregate by ID only
    // public DriverId? AssignedDriverId { get; private set; }

    public IReadOnlyCollection<MaintenanceSchedule> MaintenanceSchedules =>
        _maintenanceSchedules.AsReadOnly();

    // ------------------------
    // Constructors
    // ------------------------
    private Vehicle() { } // EF Core

    private Vehicle(VehicleId id, VehiclePlateNumber plate, VehicleSpecification spec)
    {
        Id = id;
        PlateNumber = plate;
        Specification = spec;
        Status = VehicleStatus.Available;
        CreatedAt = DateTime.UtcNow;

        AddDomainEvent(new VehicleRegisteredEvent(Id));
    }

    // ------------------------
    // Factory
    // ------------------------
    public static Result<Vehicle> Register(
        VehiclePlateNumber plate,
        VehicleSpecification spec,
        IVehicleUniquenessChecker uniquenessChecker)
    {
        if (!uniquenessChecker.IsPlateUnique(plate))
            return Result<Vehicle>.Failure(VehicleErrors.PlateAlreadyExists);

        return Result<Vehicle>.Success(new Vehicle(VehicleId.New(), plate, spec));
    }

    // ------------------------
    // Domain Behavior Methods
    // ------------------------

    // public Result AssignDriver(DriverId driverId)
    // {
    //     // Re-introduce VehicleMustBeAvailableRule here when the Driver
    //     // module is enhanced (it was removed as dead code meanwhile).
    //     AssignedDriverId = driverId;
    //     Status = VehicleStatus.InUse;
    //
    //     AddDomainEvent(new VehicleStatusChangedEvent(Id, Status));
    //     return Result.Success();
    // }

    /// <summary>
    /// Schedules future maintenance. Scheduling does NOT change the
    /// current status: a vehicle booked for maintenance next week is
    /// still Available/InUse today. It only blocks Retired vehicles.
    /// The vehicle becomes InMaintenance when it actually enters the
    /// shop (<see cref="UpdateStatus"/>) and returns to Available via
    /// <see cref="CompleteMaintenance"/>.
    /// </summary>
    public Result ScheduleMaintenance(DateTime date, MaintenanceDescription description)
    {
        var retiredRule = new VehicleCannotBeRetiredRule(Status);
        if (retiredRule.IsBroken())
            return Result.Failure(retiredRule.Error);

        var scheduleResult = MaintenanceSchedule.Create(description, date);
        if (scheduleResult.IsFailure)
            return scheduleResult;

        _maintenanceSchedules.Add(scheduleResult.Value);
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new MaintenanceScheduledEvent(Id, scheduleResult.Value.Id, date));
        return Result.Success();
    }

    /// <summary>
    /// Marks a scheduled maintenance as performed. Previously dead
    /// aggregate behavior (nothing ever called MarkAsCompleted): this
    /// is the missing use case that closes the maintenance loop and
    /// gives <see cref="MaintenancePerformedEvent"/> a real purpose.
    /// </summary>
    public Result CompleteMaintenance(Guid scheduleId, MaintenanceRemarks? remarks)
    {
        var schedule = _maintenanceSchedules.FirstOrDefault(s => s.Id == scheduleId);
        if (schedule is null)
            return Result.Failure(MaintenanceErrors.ScheduleNotFound(scheduleId));

        if (Status == VehicleStatus.Retired)
            return Result.Failure(VehicleErrors.VehicleAlreadyRetired);

        var completeResult = schedule.MarkAsCompleted(remarks, DateTime.UtcNow);
        if (completeResult.IsFailure)
            return completeResult;

        // A vehicle that was InMaintenance for this work is back in service.
        if (Status == VehicleStatus.InMaintenance)
            Status = VehicleStatus.Available;

        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new MaintenancePerformedEvent(Id, scheduleId));
        return Result.Success();
    }

    /// <summary>
    /// Moves a not-yet-completed maintenance schedule to a new date.
    /// Previously dead aggregate behavior (Reschedule was unreachable).
    /// </summary>
    public Result RescheduleMaintenance(Guid scheduleId, DateTime newDate)
    {
        var schedule = _maintenanceSchedules.FirstOrDefault(s => s.Id == scheduleId);
        if (schedule is null)
            return Result.Failure(MaintenanceErrors.ScheduleNotFound(scheduleId));

        var rescheduleResult = schedule.Reschedule(newDate);
        if (rescheduleResult.IsFailure)
            return rescheduleResult;

        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new MaintenanceScheduledEvent(Id, scheduleId, newDate));
        return Result.Success();
    }

    public Result RecordFuelConsumption(FuelConsumption consumption)
    {
        var rule = new FuelConsumptionMustBePositiveRule(consumption);
        if (rule.IsBroken())
            return Result.Failure(rule.Error);

        FuelConsumption = consumption;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new FuelConsumptionRecordedEvent(Id, consumption.Liters, consumption.OdometerReading));
        return Result.Success();
    }

    public Result Retire()
    {
        var rule = new VehicleCannotBeRetiredRule(Status);
        if (rule.IsBroken())
            return Result.Failure(rule.Error);

        Status = VehicleStatus.Retired;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new VehicleRetiredEvent(Id));
        return Result.Success();
    }

    /// <summary>
    /// Enforces the status state machine: only transitions listed in
    /// <see cref="AllowedTransitions"/> are accepted, so a Retired
    /// vehicle can no longer be flipped back to Available by accident.
    /// </summary>
    public Result UpdateStatus(VehicleStatus newStatus)
    {
        if (!AllowedTransitions[Status].Contains(newStatus))
            return Result.Failure(VehicleErrors.InvalidStatusTransition(Status, newStatus));

        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new VehicleStatusChangedEvent(Id, newStatus));
        return Result.Success();
    }

    // ------------------------
    // Soft Delete
    // ------------------------
    /// <summary>
    /// Uses the base <see cref="Entity{TId}.Delete"/> state instead of a
    /// private duplicate copy, so the soft-delete query filter and the
    /// SoftDeleteInterceptor observe the same flag.
    /// </summary>
    public void SoftDelete(string? deletedBy = null)
    {
        Delete();
        UpdatedBy = deletedBy;
    }
}

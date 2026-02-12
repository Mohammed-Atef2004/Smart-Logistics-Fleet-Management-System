using Domain.SharedKernel;
using Domain.Vehicles.Enums;
using Domain.Vehicles.Errors;
using Domain.Vehicles.Events;
using Domain.Vehicles.Rules;
using Domain.Vehicles.ValueObjects;

namespace Domain.Vehicles;

public class Vehicle : AggregateRoot<VehicleId>, IAudiatable, ISoftDeletable
{
    // ------------------------
    // Private fields / state
    // ------------------------
    private readonly List<MaintenanceSchedule> _maintenanceSchedules = new();

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

    // Audit
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public string? CreatedBy { get; private set; }
    public string? UpdatedBy { get; private set; }

    // Soft delete
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }

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
    //     var ruleResult = CheckRule(new VehicleMustBeAvailableRule(Status));
    //     if (ruleResult.IsFailure)
    //         return ruleResult;
    //
    //     AssignedDriverId = driverId;
    //     Status = VehicleStatus.InUse;
    //
    //     AddDomainEvent(new VehicleStatusChangedEvent(Id, Status));
    //     return Result.Success();
    // }

    public Result ScheduleMaintenance(DateTime date, MaintenanceDescription description)
    {
        var retiredRule = new VehicleCannotBeRetiredRule(Status);
        if (retiredRule.IsBroken())
            return Result.Failure(retiredRule.Error);

        var maintenanceResult = MaintenanceSchedule.Create(description, date);
        if (maintenanceResult.IsFailure)
            return maintenanceResult;

        _maintenanceSchedules.Add(maintenanceResult.Value);
        Status = VehicleStatus.InMaintenance;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new MaintenanceScheduledEvent(Id, date));
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

    public Result UpdateStatus(VehicleStatus newStatus)
    {
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new VehicleStatusChangedEvent(Id, newStatus));
        return Result.Success();
    }

    // ------------------------
    // Soft Delete
    // ------------------------
    public void SoftDelete(string? deletedBy = null)
    {
        IsDeleted = true;
        DeletedAtUtc = DateTime.UtcNow;
        UpdatedBy = deletedBy;
    }
}

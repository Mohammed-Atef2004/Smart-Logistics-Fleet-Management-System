using Common.Domain;
using Domain.Common;
using Domain.Vehicles.Enums;
using Domain.Vehicles.Errors;
using Domain.Vehicles.Events;
using Domain.Vehicles.Rules;
using Domain.Vehicles.ValueObjects;

namespace Domain.Vehicles;

public class Vehicle : AggregateRoot<VehicleId>
{
    // ------------------------
    // Private fields / state
    // ------------------------
    private readonly List<MaintenanceSchedule> _maintenanceSchedules = new();

    // ------------------------
    // Properties
    // ------------------------
    public VehiclePlateNumber PlateNumber { get; private set; }
    public VehicleSpecification Specification { get; private set; }
    public FuelConsumption? FuelConsumption { get; private set; }
    public VehicleStatus Status { get; private set; }
    //public DriverId? AssignedDriverId { get; private set; }

    public IReadOnlyCollection<MaintenanceSchedule> MaintenanceSchedules =>
        _maintenanceSchedules.AsReadOnly();

    public DateTime CreatedAt => throw new NotImplementedException();

    public DateTime? UpdatedAt => throw new NotImplementedException();

    public string? CreatedBy => throw new NotImplementedException();

    public string? UpdatedBy => throw new NotImplementedException();

    public bool IsDeleted => throw new NotImplementedException();

    public DateTime? DeletedAtUtc => throw new NotImplementedException();

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

    //public Result AssignDriver(DriverId driverId)
    //{
    //    var ruleResult = CheckRule(new VehicleMustBeAvailableRule(Status));
    //    if (ruleResult.IsFailure)
    //        return ruleResult;

    //    AssignedDriverId = driverId;
    //    Status = VehicleStatus.InUse;

    //    AddDomainEvent(new VehicleStatusChangedEvent(Id, Status));
    //    return Result.Success();
    //}

    public Result ScheduleMaintenance(DateTime date, MaintenanceDescription description)
    {
        // BusinessRule: Vehicle must not be retired to schedule maintenance
        var retiredRule = new VehicleCannotBeRetiredRule(Status);
        if (retiredRule.IsBroken())
            return Result.Failure(retiredRule.Error);

        // Create MaintenanceSchedule safely with Result object
        var maintenanceResult = MaintenanceSchedule.Create(description, date);
        if (maintenanceResult.IsFailure)
            return maintenanceResult;

        _maintenanceSchedules.Add(maintenanceResult.Value);
        Status = VehicleStatus.InMaintenance;

        AddDomainEvent(new MaintenanceScheduledEvent(Id, date));
        return Result.Success();
    }

    public Result RecordFuelConsumption(FuelConsumption consumption)
    {
        var rule = new FuelConsumptionMustBePositiveRule(consumption);
        if (rule.IsBroken())
            return Result.Failure(rule.Error);

        FuelConsumption = consumption;
        return Result.Success();
    }

    public Result Retire()
    {
        var rule = new VehicleCannotBeRetiredRule(Status);
        if (rule.IsBroken())
            return Result.Failure(rule.Error);

        Status = VehicleStatus.Retired;
        AddDomainEvent(new VehicleRetiredEvent(Id));

        return Result.Success();
    }

    public Result UpdateStatus(VehicleStatus newStatus)
    {
        Status = newStatus;
        AddDomainEvent(new VehicleStatusChangedEvent(Id, newStatus));
        return Result.Success();
    }



  
}

using Application.Features.Vehicle.Commands.CompleteMaintenance;
using Application.Features.Vehicle.Commands.RecordFuelConsumption;
using Application.Features.Vehicle.Commands.RegisterNewVehicle;
using Application.Features.Vehicle.Commands.RescheduleMaintenance;
using Application.Features.Vehicle.Commands.RetireVehicle;
using Application.Features.Vehicle.Commands.ScheduleMaintenance;
using Application.Features.Vehicle.Commands.UpdateVehicleStatus;
using Application.Features.Vehicle.Queries.GetById;
using Application.Tests.Fakes;
using Domain.SharedKernel;
using Domain.Vehicles;
using Domain.Vehicles.Enums;
using Domain.Vehicles.Errors;
using Domain.Vehicles.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using VehicleAggregate = Domain.Vehicles.Vehicle;

namespace Application.Tests.Features.Vehicle;

/// <summary>
/// Handler-level use-case tests for the Vehicle module (fakes instead of
/// EF Core). Covers the golden paths plus every guard the scan added:
/// value-object failure checks, VehicleErrors.NotFound, the status
/// state machine, and the CompleteMaintenance / RescheduleMaintenance
/// use cases that were previously missing.
/// </summary>
public class VehicleHandlerTests
{
    private readonly FakeVehicleRepository _repo = new();
    private readonly FakeUnitOfWork _uow = new();

    private VehicleAggregate NewVehicleInRepo()
    {
        var vehicle = VehicleAggregate.Register(
            VehiclePlateNumber.Create("ABC-123").Value,
            VehicleSpecification.Create("Toyota", 2024, "Corolla").Value,
            _repo).Value;
        _repo.AddAsync(vehicle).Wait();
        return vehicle;
    }

    // ───────────────── RegisterNewVehicleCommandHandler ─────────────────

    [Fact]
    public async Task Register_ValidCommand_Should_AddVehicle_And_Save()
    {
        var handler = new RegisterNewVehicleCommandHandler(_uow, _repo, _repo, NullLogger<RegisterNewVehicleCommandHandler>.Instance);

        var result = await handler.Handle(
            new RegisterNewVehicleCommand("XYZ-999", "Honda", 2023, "Petrol"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _repo.Added.Should().HaveCount(1);
        _uow.SaveCount.Should().Be(1);
        result.Value.Value.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Register_DuplicatePlate_Should_Fail_Without_Adding()
    {
        NewVehicleInRepo(); // occupies ABC-123
        var handler = new RegisterNewVehicleCommandHandler(_uow, _repo, _repo, NullLogger<RegisterNewVehicleCommandHandler>.Instance);

        var result = await handler.Handle(
            new RegisterNewVehicleCommand("ABC-123", "Honda", 2023, "Petrol"), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(VehicleErrors.PlateAlreadyExists);
        _uow.SaveCount.Should().Be(0);
    }

    [Fact]
    public async Task Register_InvalidPlate_Should_Fail()
    {
        // Regression: the old handler fed an unchecked plate into the
        // aggregate (potential invalid state).
        var handler = new RegisterNewVehicleCommandHandler(_uow, _repo, _repo, NullLogger<RegisterNewVehicleCommandHandler>.Instance);

        var result = await handler.Handle(
            new RegisterNewVehicleCommand("   ", "Honda", 2023, "Petrol"), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(VehicleErrors.InvalidPlateNumber);
        _repo.Added.Should().BeEmpty();
    }

    [Fact]
    public async Task Register_InvalidSpecification_Should_Fail()
    {
        var handler = new RegisterNewVehicleCommandHandler(_uow, _repo, _repo, NullLogger<RegisterNewVehicleCommandHandler>.Instance);

        var result = await handler.Handle(
            new RegisterNewVehicleCommand("XYZ-999", "Honda", 1700, "Petrol"), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("VehicleSpecification.YearOutOfRange");
        _repo.Added.Should().BeEmpty();
    }

    // ───────────────── ScheduleMaintenanceCommandHandler ─────────────────

    [Fact]
    public async Task ScheduleMaintenance_VehicleNotFound_Should_Return_VehicleErrors_NotFound()
    {
        var handler = new ScheduleMaintenanceCommandHandler(_uow, _repo);

        var result = await handler.Handle(
            new ScheduleMaintenanceCommand(Guid.NewGuid(), DateTime.UtcNow.AddDays(3), "Oil Change"),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Vehicle.NotFound");
    }

    [Fact]
    public async Task ScheduleMaintenance_ValidCommand_Should_AddSchedule_And_Save()
    {
        var vehicle = NewVehicleInRepo();
        var handler = new ScheduleMaintenanceCommandHandler(_uow, _repo);

        var result = await handler.Handle(
            new ScheduleMaintenanceCommand(vehicle.Id.Value, DateTime.UtcNow.AddDays(3), "Oil Change"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        vehicle.MaintenanceSchedules.Should().HaveCount(1);
        _uow.SaveCount.Should().Be(1);
    }

    // ───────────────── UpdateVehicleStatusHandler ─────────────────

    [Fact]
    public async Task UpdateStatus_VehicleNotFound_Should_Fail()
    {
        var handler = new UpdateVehicleStatusHandler(_uow, _repo);

        var result = await handler.Handle(
            new UpdateVehicleStatusCommand(Guid.NewGuid(), VehicleStatus.InUse), CancellationToken.None);

        result.Error.Code.Should().Be("Vehicle.NotFound");
    }

    [Fact]
    public async Task UpdateStatus_Retired_To_Available_Should_Be_Rejected_By_The_StateMachine()
    {
        var vehicle = NewVehicleInRepo();
        vehicle.Retire();
        var handler = new UpdateVehicleStatusHandler(_uow, _repo);

        var result = await handler.Handle(
            new UpdateVehicleStatusCommand(vehicle.Id.Value, VehicleStatus.Available), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Vehicle.InvalidStatusTransition");
        _uow.SaveCount.Should().Be(0);
    }

    // ───────────────── CompleteMaintenanceCommandHandler ─────────────────

    [Fact]
    public async Task CompleteMaintenance_UnknownSchedule_Should_Fail()
    {
        var vehicle = NewVehicleInRepo();
        var handler = new CompleteMaintenanceCommandHandler(_uow, _repo);

        var result = await handler.Handle(
            new CompleteMaintenanceCommand(vehicle.Id.Value, Guid.NewGuid(), null), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Maintenance.ScheduleNotFound");
    }

    [Fact]
    public async Task CompleteMaintenance_ValidCommand_Should_Complete_And_Save()
    {
        var vehicle = NewVehicleInRepo();
        vehicle.ScheduleMaintenance(DateTime.UtcNow.AddDays(3), MaintenanceDescription.Create("Oil Change").Value);
        var scheduleId = vehicle.MaintenanceSchedules.Single().Id;
        var handler = new CompleteMaintenanceCommandHandler(_uow, _repo);

        var result = await handler.Handle(
            new CompleteMaintenanceCommand(vehicle.Id.Value, scheduleId, "All good."), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        vehicle.MaintenanceSchedules.Single().IsCompleted.Should().BeTrue();
        _uow.SaveCount.Should().Be(1);
    }

    // ───────────────── RescheduleMaintenanceCommandHandler ─────────────────

    [Fact]
    public async Task RescheduleMaintenance_ValidCommand_Should_Move_Date()
    {
        var vehicle = NewVehicleInRepo();
        vehicle.ScheduleMaintenance(DateTime.UtcNow.AddDays(3), MaintenanceDescription.Create("Oil Change").Value);
        var scheduleId = vehicle.MaintenanceSchedules.Single().Id;
        var handler = new RescheduleMaintenanceCommandHandler(_uow, _repo);

        var result = await handler.Handle(
            new RescheduleMaintenanceCommand(vehicle.Id.Value, scheduleId, DateTime.UtcNow.AddDays(12)),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        vehicle.MaintenanceSchedules.Single().ScheduledDate.Should().BeCloseTo(DateTime.UtcNow.AddDays(12), TimeSpan.FromSeconds(1));
    }

    // ───────────────── RecordFuelConsumptionHandler ─────────────────

    [Fact]
    public async Task RecordFuel_VehicleNotFound_Should_Fail()
    {
        var handler = new RecordFuelConsumptionCommandHandler(_uow, _repo);

        var result = await handler.Handle(
            new RecordFuelConsumptionCommand(Guid.NewGuid(), 50, 1000), CancellationToken.None);

        result.Error.Code.Should().Be("Vehicle.NotFound");
    }

    [Fact]
    public async Task RecordFuel_ValidCommand_Should_Record_And_Save()
    {
        var vehicle = NewVehicleInRepo();
        var handler = new RecordFuelConsumptionCommandHandler(_uow, _repo);

        var result = await handler.Handle(
            new RecordFuelConsumptionCommand(vehicle.Id.Value, 45.5m, 12500), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        vehicle.FuelConsumption!.Liters.Should().Be(45.5m);
        _uow.SaveCount.Should().Be(1);
    }

    // ───────────────── RetireVehicleCommandHandler ─────────────────

    [Fact]
    public async Task Retire_VehicleNotFound_Should_Fail()
    {
        var handler = new RetireVehicleCommandHandler(_uow, _repo);

        var result = await handler.Handle(new RetireVehicleCommand(Guid.NewGuid()), CancellationToken.None);

        result.Error.Code.Should().Be("Vehicle.NotFound");
    }

    // ───────────────── GetVehicleDetailsHandler ─────────────────

    [Fact]
    public async Task GetDetails_VehicleNotFound_Should_Fail()
    {
        var handler = new GetVehicleDetailsHandler(_repo);

        var result = await handler.Handle(new GetVehicleDetailsQuery(Guid.NewGuid()), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Vehicle.NotFound");
    }

    [Fact]
    public async Task GetDetails_Should_Compute_LastMaintenanceDate_From_Completed_Schedules()
    {
        // Regression: the old handler always returned null here.
        var vehicle = NewVehicleInRepo();
        vehicle.ScheduleMaintenance(DateTime.UtcNow.AddDays(3), MaintenanceDescription.Create("Oil Change").Value);
        vehicle.CompleteMaintenance(vehicle.MaintenanceSchedules.Single().Id, null);
        vehicle.ScheduleMaintenance(DateTime.UtcNow.AddDays(9), MaintenanceDescription.Create("Brakes").Value); // open schedule
        var handler = new GetVehicleDetailsHandler(_repo);

        var result = await handler.Handle(new GetVehicleDetailsQuery(vehicle.Id.Value), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.LastMaintenanceDate.Should().NotBeNull();
        result.Value.LastMaintenanceDate.Should().Be(vehicle.MaintenanceSchedules.First(s => s.IsCompleted).CompletedAt);
    }

    [Fact]
    public async Task GetDetails_Without_Completed_Maintenance_Should_Return_Null_LastMaintenanceDate()
    {
        var vehicle = NewVehicleInRepo();
        var handler = new GetVehicleDetailsHandler(_repo);

        var result = await handler.Handle(new GetVehicleDetailsQuery(vehicle.Id.Value), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.LastMaintenanceDate.Should().BeNull();
    }
}

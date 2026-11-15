using Domain.SharedKernel;
using Domain.Vehicles;
using Domain.Vehicles.Enums;
using Domain.Vehicles.Errors;
using Domain.Vehicles.Events;
using Domain.Vehicles.ValueObjects;
using FluentAssertions;

namespace Domain.Tests;

public class VehicleTests
{
    private readonly VehiclePlateNumber _plate = VehiclePlateNumber.Create("ABC-123").Value;
    private readonly VehicleSpecification _spec = VehicleSpecification.Create("Toyota", 2024, "Corolla").Value;

    private Vehicle NewVehicle() =>
        Vehicle.Register(_plate, _spec, new FakeVehicleUniquenessChecker(isUnique: true)).Value;

    private static MaintenanceDescription Description(string text = "Oil Change") =>
        MaintenanceDescription.Create(text).Value;

    // ───────────────────────── Registration ─────────────────────────

    [Fact]
    public void Register_Should_Create_Vehicle_When_Plate_Is_Unique()
    {
        var result = Vehicle.Register(_plate, _spec, new FakeVehicleUniquenessChecker(true));

        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(VehicleStatus.Available);
        result.Value.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        result.Value.DomainEvents.Should().ContainSingle(e => e is VehicleRegisteredEvent);
    }

    [Fact]
    public void Register_Should_Fail_When_Plate_Is_Not_Unique()
    {
        var result = Vehicle.Register(_plate, _spec, new FakeVehicleUniquenessChecker(false));

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(VehicleErrors.PlateAlreadyExists);
    }

    [Fact]
    public void VehicleRegisteredEvent_Should_Carry_VehicleId_InItsOwnProperty()
    {
        // Guards the CS0108 class of bugs: the payload must live in a
        // property NOT named "Id" and must hold the vehicle id.
        var vehicle = NewVehicle();

        var @event = vehicle.DomainEvents.OfType<VehicleRegisteredEvent>().Single();
        @event.VehicleId.Should().Be(vehicle.Id);
        @event.VehicleId.Value.Should().NotBe(@event.Id); // Id is the event's own GUID
    }

    // ───────────────────────── Status state machine ─────────────────────────

    [Fact]
    public void UpdateStatus_Should_Allow_Available_To_InUse()
    {
        var vehicle = NewVehicle();

        var result = vehicle.UpdateStatus(VehicleStatus.InUse);

        result.IsSuccess.Should().BeTrue();
        vehicle.Status.Should().Be(VehicleStatus.InUse);
        vehicle.DomainEvents.Should().Contain(e => e is VehicleStatusChangedEvent);
    }

    [Fact]
    public void UpdateStatus_Should_Allow_Available_To_InMaintenance_And_To_Retired()
    {
        var v1 = NewVehicle();
        v1.UpdateStatus(VehicleStatus.InMaintenance).IsSuccess.Should().BeTrue();
        v1.Status.Should().Be(VehicleStatus.InMaintenance);

        var v2 = NewVehicle();
        v2.UpdateStatus(VehicleStatus.Retired).IsSuccess.Should().BeTrue();
        v2.Status.Should().Be(VehicleStatus.Retired);
    }

    [Fact]
    public void UpdateStatus_Should_Reject_Retired_To_Anything()
    {
        var vehicle = NewVehicle();
        vehicle.Retire();

        foreach (var target in new[] { VehicleStatus.Available, VehicleStatus.InUse, VehicleStatus.InMaintenance })
        {
            var result = vehicle.UpdateStatus(target);

            result.IsFailure.Should().BeTrue("Retired is terminal");
            result.Error.Should().Be(VehicleErrors.InvalidStatusTransition(VehicleStatus.Retired, target));
            vehicle.Status.Should().Be(VehicleStatus.Retired);
        }
    }

    [Fact]
    public void UpdateStatus_Should_Reject_NoOp_Transition()
    {
        var vehicle = NewVehicle();

        var result = vehicle.UpdateStatus(VehicleStatus.Available);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void UpdateStatus_Should_Allow_InUse_Back_To_Available()
    {
        var vehicle = NewVehicle();
        vehicle.UpdateStatus(VehicleStatus.InUse);

        var result = vehicle.UpdateStatus(VehicleStatus.Available);

        result.IsSuccess.Should().BeTrue();
        vehicle.Status.Should().Be(VehicleStatus.Available);
    }

    [Fact]
    public void Retire_Should_Set_Status_And_Retire_Twice_Should_Fail()
    {
        var vehicle = NewVehicle();

        var first = vehicle.Retire();
        first.IsSuccess.Should().BeTrue();
        vehicle.Status.Should().Be(VehicleStatus.Retired);
        vehicle.DomainEvents.Should().Contain(e => e is VehicleRetiredEvent);

        var second = vehicle.Retire();
        second.IsFailure.Should().BeTrue();
        second.Error.Should().Be(VehicleErrors.VehicleAlreadyRetired);
    }

    // ───────────────────────── ScheduleMaintenance ─────────────────────────

    [Fact]
    public void ScheduleMaintenance_Should_AddSchedule_Without_Changing_Status()
    {
        // Regression: the old code forced Status = InMaintenance even for
        // a date two weeks out (a vehicle booked for the future is still
        // usable today) and overrode InUse.
        var vehicle = NewVehicle();
        var date = DateTime.UtcNow.AddDays(10);

        var result = vehicle.ScheduleMaintenance(date, Description());

        result.IsSuccess.Should().BeTrue();
        vehicle.Status.Should().Be(VehicleStatus.Available);
        vehicle.MaintenanceSchedules.Should().HaveCount(1);
        vehicle.MaintenanceSchedules.Single().ScheduledDate.Should().BeCloseTo(date, TimeSpan.FromSeconds(1));
        vehicle.DomainEvents.Should().Contain(e => e is MaintenanceScheduledEvent);
    }

    [Fact]
    public void ScheduleMaintenance_Should_Reject_Past_Date()
    {
        var vehicle = NewVehicle();

        var result = vehicle.ScheduleMaintenance(DateTime.UtcNow.AddDays(-1), Description());

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(MaintenanceErrors.DateInPast);
        vehicle.MaintenanceSchedules.Should().BeEmpty();
    }

    [Fact]
    public void ScheduleMaintenance_Should_Reject_Retired_Vehicle()
    {
        var vehicle = NewVehicle();
        vehicle.Retire();

        var result = vehicle.ScheduleMaintenance(DateTime.UtcNow.AddDays(10), Description());

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(VehicleErrors.VehicleAlreadyRetired);
    }

    // ───────────────────────── CompleteMaintenance ─────────────────────────

    [Fact]
    public void CompleteMaintenance_Should_CompleteSchedule_RaiseEvent_And_Return_InMaintenance_Vehicle_To_Available()
    {
        var vehicle = NewVehicle();
        vehicle.ScheduleMaintenance(DateTime.UtcNow.AddDays(3), Description());
        vehicle.UpdateStatus(VehicleStatus.InMaintenance); // vehicle entered the shop
        var scheduleId = vehicle.MaintenanceSchedules.Single().Id;
        var remarks = MaintenanceRemarks.Create("Replaced brake pads.").Value;

        var result = vehicle.CompleteMaintenance(scheduleId, remarks);

        result.IsSuccess.Should().BeTrue();
        var schedule = vehicle.MaintenanceSchedules.Single();
        schedule.IsCompleted.Should().BeTrue();
        schedule.CompletedAt.Should().NotBeNull();
        schedule.Remarks.Should().Be(remarks);
        vehicle.Status.Should().Be(VehicleStatus.Available);
        vehicle.DomainEvents.OfType<MaintenancePerformedEvent>()
            .Should().ContainSingle(p => p.ScheduleId == scheduleId);
    }

    [Fact]
    public void CompleteMaintenance_When_Vehicle_Was_InUse_Should_Keep_InUse_Status()
    {
        var vehicle = NewVehicle();
        vehicle.ScheduleMaintenance(DateTime.UtcNow.AddDays(3), Description());
        vehicle.UpdateStatus(VehicleStatus.InUse);
        var scheduleId = vehicle.MaintenanceSchedules.Single().Id;

        var result = vehicle.CompleteMaintenance(scheduleId, null);

        result.IsSuccess.Should().BeTrue();
        vehicle.Status.Should().Be(VehicleStatus.InUse); // the driver is still on the road
    }

    [Fact]
    public void CompleteMaintenance_Should_Reject_Completing_Twice()
    {
        var vehicle = NewVehicle();
        vehicle.ScheduleMaintenance(DateTime.UtcNow.AddDays(3), Description());
        var scheduleId = vehicle.MaintenanceSchedules.Single().Id;
        vehicle.CompleteMaintenance(scheduleId, null);

        var result = vehicle.CompleteMaintenance(scheduleId, null);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(MaintenanceErrors.AlreadyCompleted);
    }

    [Fact]
    public void CompleteMaintenance_Should_Reject_Unknown_Schedule()
    {
        var vehicle = NewVehicle();

        var result = vehicle.CompleteMaintenance(Guid.NewGuid(), null);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Maintenance.ScheduleNotFound");
    }

    [Fact]
    public void CompleteMaintenance_Should_Reject_Retired_Vehicle()
    {
        var vehicle = NewVehicle();
        vehicle.ScheduleMaintenance(DateTime.UtcNow.AddDays(3), Description());
        vehicle.Retire();
        var scheduleId = vehicle.MaintenanceSchedules.Single().Id;

        var result = vehicle.CompleteMaintenance(scheduleId, null);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(VehicleErrors.VehicleAlreadyRetired);
    }

    // ───────────────────────── RescheduleMaintenance ─────────────────────────

    [Fact]
    public void RescheduleMaintenance_Should_Move_Date_And_Raise_Event()
    {
        var vehicle = NewVehicle();
        vehicle.ScheduleMaintenance(DateTime.UtcNow.AddDays(3), Description());
        var scheduleId = vehicle.MaintenanceSchedules.Single().Id;
        var newDate = DateTime.UtcNow.AddDays(14);

        var result = vehicle.RescheduleMaintenance(scheduleId, newDate);

        result.IsSuccess.Should().BeTrue();
        vehicle.MaintenanceSchedules.Single().ScheduledDate.Should().BeCloseTo(newDate, TimeSpan.FromSeconds(1));
        vehicle.DomainEvents.Should().Contain(e => e is MaintenanceScheduledEvent);
    }

    [Fact]
    public void RescheduleMaintenance_Should_Reject_Completed_Schedule()
    {
        var vehicle = NewVehicle();
        vehicle.ScheduleMaintenance(DateTime.UtcNow.AddDays(3), Description());
        var scheduleId = vehicle.MaintenanceSchedules.Single().Id;
        vehicle.CompleteMaintenance(scheduleId, null);

        var result = vehicle.RescheduleMaintenance(scheduleId, DateTime.UtcNow.AddDays(9));

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(MaintenanceErrors.CannotRescheduleCompleted);
    }

    [Fact]
    public void RescheduleMaintenance_Should_Reject_Past_Date_And_Unknown_Schedule()
    {
        var vehicle = NewVehicle();
        vehicle.ScheduleMaintenance(DateTime.UtcNow.AddDays(3), Description());
        var scheduleId = vehicle.MaintenanceSchedules.Single().Id;

        vehicle.RescheduleMaintenance(scheduleId, DateTime.UtcNow.AddDays(-2))
            .Error.Should().Be(MaintenanceErrors.DateInPast);
        vehicle.RescheduleMaintenance(Guid.NewGuid(), DateTime.UtcNow.AddDays(9))
            .Error.Code.Should().Be("Maintenance.ScheduleNotFound");
    }

    // ───────────────────────── Fuel consumption ─────────────────────────

    [Fact]
    public void RecordFuelConsumption_Should_Succeed_And_Raise_Event()
    {
        var vehicle = NewVehicle();
        var fuel = FuelConsumption.Create(50, 1200).Value;

        var result = vehicle.RecordFuelConsumption(fuel);

        result.IsSuccess.Should().BeTrue();
        vehicle.FuelConsumption.Should().Be(fuel);
        var @event = vehicle.DomainEvents.OfType<FuelConsumptionRecordedEvent>().Single();
        @event.VehicleId.Should().Be(vehicle.Id);
        @event.Liters.Should().Be(50);
        @event.OdometerReading.Should().Be(1200);
    }

    [Fact]
    public void FuelConsumption_Create_Should_Reject_NonPositive_Values()
    {
        FuelConsumption.Create(0, 1000).IsFailure.Should().BeTrue();
        FuelConsumption.Create(-5, 1000).IsFailure.Should().BeTrue();
        FuelConsumption.Create(50, 0).IsFailure.Should().BeTrue();
    }

    // ───────────────────────── Soft delete ─────────────────────────

    [Fact]
    public void SoftDelete_Should_Mark_Vehicle_As_Deleted()
    {
        var vehicle = NewVehicle();

        vehicle.SoftDelete("admin");

        vehicle.IsDeleted.Should().BeTrue();
        vehicle.DeletedAtUtc.Should().NotBeNull();
        vehicle.UpdatedBy.Should().Be("admin");
    }

    [Fact]
    public void DomainEvents_Should_Be_Cleared()
    {
        var vehicle = NewVehicle();

        vehicle.ClearDomainEvents();

        vehicle.DomainEvents.Should().BeEmpty();
    }

    // ───────────────────────── Maintenance value objects ─────────────────────────

    [Fact]
    public void MaintenanceDescription_Create_Should_Enforce_Invariants()
    {
        MaintenanceDescription.Create("").IsFailure.Should().BeTrue();
        MaintenanceDescription.Create("   ").IsFailure.Should().BeTrue();
        MaintenanceDescription.Create(new string('x', 201)).IsFailure.Should().BeTrue();
        MaintenanceDescription.Create(new string('x', 200)).IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void MaintenanceRemarks_Create_Should_Enforce_Invariants()
    {
        MaintenanceRemarks.Create(new string('x', 501)).IsFailure.Should().BeTrue();
        MaintenanceRemarks.Create(new string('x', 500)).IsSuccess.Should().BeTrue();
        MaintenanceRemarks.Create(null).IsSuccess.Should().BeTrue();
        MaintenanceRemarks.Create("   ").Value.Value.Should().BeNull();
        MaintenanceRemarks.Create("  done  ").Value.Value.Should().Be("done");
    }
}

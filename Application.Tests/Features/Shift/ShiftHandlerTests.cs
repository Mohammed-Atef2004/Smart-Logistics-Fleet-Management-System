using Application.Features.Driver.DomainServices;
using Application.Features.Shift.Commands.AbortShift;
using Application.Features.Shift.Commands.CancelShift;
using Application.Features.Shift.Commands.CompleteShift;
using Application.Features.Shift.Commands.Create;
using Application.Features.Shift.Commands.RescheduleShift;
using Application.Features.Shift.Commands.StartShift;
using Application.Features.Shift.DTOs;
using Application.Features.Shift.EventHandlers;
using Application.Features.Shift.Mappings;
using Application.Features.Shift.Queries.GetAll;
using Application.Features.Shift.Queries.GetById;
using Application.Tests.Fakes;
using AutoMapper;
using Domain.Drivers.Enums;
using Domain.Drivers.Errors;
using Domain.Drivers.ValueObjects;
using Domain.Shifts.Enums;
using Domain.Shifts.Errors;
using Domain.Shifts.Events;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using DriverAggregate = Domain.Drivers.Driver;
using ShiftAggregate = Domain.Shifts.Shift;

namespace Application.Tests.Features.Shift;

/// <summary>
/// Handler-level use-case tests for the Shift module (fakes instead of EF
/// Core). Covers the creation path shared with the driver endpoint, the full
/// state machine, rescheduling with overlap detection, queries, mapping, and
/// the event handler that releases the driver's current shift.
/// </summary>
public class ShiftHandlerTests
{
    private readonly FakeDriverRepository _drivers = new();
    private readonly FakeShiftRepository _shifts = new();
    private readonly FakeUnitOfWork _uow = new();
    private readonly FakeDriverUniquenessChecker _driverChecker;

    public ShiftHandlerTests()
    {
        _driverChecker = new FakeDriverUniquenessChecker(_drivers);
    }

    // ---------------------------------------------------------------
    // Helpers
    // ---------------------------------------------------------------

    private DriverAggregate HiredDriver(string name = "Mohamed", string licenseNumber = "123456")
    {
        var license = DriverLicense.Create(licenseNumber, DateTime.UtcNow.AddYears(1), "B").Value;
        var driver = DriverAggregate.Hire(name, license, _driverChecker).Value;
        _drivers.AddAsync(driver).Wait();
        return driver;
    }

    private CreateShiftCommandHandler CreateHandler()
        => new(_uow, new DriverShiftService(_drivers, _shifts));

    private async Task<ShiftAggregate> CreatedShiftAsync(DriverAggregate driver)
    {
        var start = DateTime.UtcNow.AddHours(1);
        var result = await CreateHandler().Handle(
            new CreateShiftCommand(driver.Id.Value, start, start.AddHours(4)),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        return (await _shifts.GetByIdAsync(new Domain.Shifts.ValueObjects.ShiftId(result.Value)))!;
    }

    private static IMapper Mapper()
        => new MapperConfiguration(
                cfg => cfg.AddProfile<ShiftProfile>(),
                NullLoggerFactory.Instance)
            .CreateMapper();

    // ---------------------------------------------------------------
    // Create
    // ---------------------------------------------------------------

    [Fact]
    public async Task Create_Should_Add_Shift_And_Assign_Driver()
    {
        var driver = HiredDriver();

        var result = await CreateHandler().Handle(
            new CreateShiftCommand(driver.Id.Value, DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(5)),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBe(Guid.Empty);
        _shifts.Added.Should().ContainSingle();
        driver.CurrentShiftId.Should().NotBeNull();
        driver.CurrentShiftId!.Value.Value.Should().Be(result.Value);
        _uow.SaveCount.Should().Be(1);
    }

    [Fact]
    public async Task Create_Should_Fail_When_Driver_Does_Not_Exist()
    {
        var result = await CreateHandler().Handle(
            new CreateShiftCommand(Guid.NewGuid(), DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(5)),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Driver.NotFound");
        _shifts.Added.Should().BeEmpty();
        _uow.SaveCount.Should().Be(0);
    }

    [Fact]
    public async Task Create_Should_Fail_When_Driver_Is_Suspended()
    {
        var driver = HiredDriver();
        driver.Suspend(DriverSuspensionReason.Misconduct);

        var result = await CreateHandler().Handle(
            new CreateShiftCommand(driver.Id.Value, DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(5)),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DriverErrors.NotActive);
        _shifts.Added.Should().BeEmpty();
    }

    [Fact]
    public async Task Create_Should_Fail_When_Driver_Already_Has_A_Shift()
    {
        var driver = HiredDriver();
        await CreatedShiftAsync(driver);

        var result = await CreateHandler().Handle(
            new CreateShiftCommand(driver.Id.Value, DateTime.UtcNow.AddHours(10), DateTime.UtcNow.AddHours(12)),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DriverErrors.AlreadyInShift);
        _shifts.Added.Should().ContainSingle();
    }

    [Fact]
    public async Task Create_Should_Fail_When_Window_Overlaps_An_Existing_Shift()
    {
        var driver = HiredDriver();
        var start = DateTime.UtcNow.AddHours(1);
        _shifts.Seed(ShiftAggregate.Create(driver.Id, start, start.AddHours(3)).Value);

        var result = await CreateHandler().Handle(
            new CreateShiftCommand(driver.Id.Value, start.AddHours(2), start.AddHours(5)),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ShiftErrors.Overlap);
        _shifts.Added.Should().BeEmpty();
        driver.CurrentShiftId.Should().BeNull();
    }

    [Fact]
    public async Task Create_Should_Return_Domain_Error_For_Invalid_Window()
    {
        var driver = HiredDriver();
        var start = DateTime.UtcNow.AddHours(2);

        var result = await CreateHandler().Handle(
            new CreateShiftCommand(driver.Id.Value, start, start.AddHours(-1)),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ShiftErrors.InvalidDuration);
        _shifts.Added.Should().BeEmpty();
    }

    // ---------------------------------------------------------------
    // State machine handlers
    // ---------------------------------------------------------------

    [Fact]
    public async Task Start_Should_Activate_Shift_And_Save()
    {
        var shift = await CreatedShiftAsync(HiredDriver());
        var handler = new StartShiftCommandHandler(_uow, _shifts);

        var result = await handler.Handle(new StartShiftCommand(shift.Id.Value), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        shift.Status.Should().Be(ShiftStatus.Active);
        _uow.SaveCount.Should().Be(2); // create + start
    }

    [Fact]
    public async Task Start_NotFound_Should_Return_NotFound()
    {
        var handler = new StartShiftCommandHandler(_uow, _shifts);

        var result = await handler.Handle(new StartShiftCommand(Guid.NewGuid()), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Shift.NotFound");
    }

    [Fact]
    public async Task Start_Should_Fail_When_Already_Active()
    {
        var shift = await CreatedShiftAsync(HiredDriver());
        shift.Start();
        var handler = new StartShiftCommandHandler(_uow, _shifts);

        var result = await handler.Handle(new StartShiftCommand(shift.Id.Value), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ShiftErrors.CannotStart);
    }

    [Fact]
    public async Task Complete_Should_Complete_Active_Shift()
    {
        var shift = await CreatedShiftAsync(HiredDriver());
        shift.Start();
        var handler = new CompleteShiftCommandHandler(_uow, _shifts);

        // Regression: the old "complete" handler called Cancel().
        var result = await handler.Handle(new CompleteShiftCommand(shift.Id.Value), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        shift.Status.Should().Be(ShiftStatus.Completed);
    }

    [Fact]
    public async Task Complete_Should_Fail_When_Planned()
    {
        var shift = await CreatedShiftAsync(HiredDriver());
        var handler = new CompleteShiftCommandHandler(_uow, _shifts);

        var result = await handler.Handle(new CompleteShiftCommand(shift.Id.Value), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ShiftErrors.CannotComplete);
    }

    [Fact]
    public async Task Cancel_Should_Cancel_Planned_Shift()
    {
        var shift = await CreatedShiftAsync(HiredDriver());
        var handler = new CancelShiftCommandHandler(_uow, _shifts);

        var result = await handler.Handle(new CancelShiftCommand(shift.Id.Value), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        shift.Status.Should().Be(ShiftStatus.Cancelled);
    }

    [Fact]
    public async Task Cancel_Should_Fail_When_Active()
    {
        var shift = await CreatedShiftAsync(HiredDriver());
        shift.Start();
        var handler = new CancelShiftCommandHandler(_uow, _shifts);

        var result = await handler.Handle(new CancelShiftCommand(shift.Id.Value), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ShiftErrors.CannotCancel);
    }

    [Fact]
    public async Task Abort_Should_Abort_Active_Shift()
    {
        var shift = await CreatedShiftAsync(HiredDriver());
        shift.Start();
        var handler = new AbortShiftCommandHandler(_uow, _shifts);

        var result = await handler.Handle(new AbortShiftCommand(shift.Id.Value), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        shift.Status.Should().Be(ShiftStatus.Aborted);
    }

    [Fact]
    public async Task Abort_Should_Fail_When_Planned()
    {
        var shift = await CreatedShiftAsync(HiredDriver());
        var handler = new AbortShiftCommandHandler(_uow, _shifts);

        var result = await handler.Handle(new AbortShiftCommand(shift.Id.Value), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ShiftErrors.CannotAbort);
    }

    // ---------------------------------------------------------------
    // Reschedule
    // ---------------------------------------------------------------

    [Fact]
    public async Task Reschedule_Should_Move_Planned_Shift()
    {
        var shift = await CreatedShiftAsync(HiredDriver());
        var newStart = DateTime.UtcNow.AddDays(1);
        var handler = new RescheduleShiftCommandHandler(_uow, _shifts);

        var result = await handler.Handle(
            new RescheduleShiftCommand(shift.Id.Value, newStart, newStart.AddHours(3)),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        shift.ShiftStart.Should().Be(newStart);
        shift.ShiftEnd.Should().Be(newStart.AddHours(3));
    }

    [Fact]
    public async Task Reschedule_Should_Exclude_Itself_From_The_Overlap_Check()
    {
        var shift = await CreatedShiftAsync(HiredDriver());
        var handler = new RescheduleShiftCommandHandler(_uow, _shifts);

        // Same window: must not conflict with itself.
        var result = await handler.Handle(
            new RescheduleShiftCommand(shift.Id.Value, shift.ShiftStart, shift.ShiftEnd),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Reschedule_Should_Fail_When_New_Window_Overlaps_Another_Shift()
    {
        var driver = HiredDriver();
        var shift = await CreatedShiftAsync(driver);
        var otherStart = DateTime.UtcNow.AddDays(1);
        _shifts.Seed(ShiftAggregate.Create(driver.Id, otherStart, otherStart.AddHours(4)).Value);
        var handler = new RescheduleShiftCommandHandler(_uow, _shifts);

        var result = await handler.Handle(
            new RescheduleShiftCommand(shift.Id.Value, otherStart.AddHours(1), otherStart.AddHours(3)),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ShiftErrors.Overlap);
    }

    [Fact]
    public async Task Reschedule_Should_Fail_When_Shift_Is_Active()
    {
        var shift = await CreatedShiftAsync(HiredDriver());
        shift.Start();
        var handler = new RescheduleShiftCommandHandler(_uow, _shifts);

        var result = await handler.Handle(
            new RescheduleShiftCommand(
                shift.Id.Value, DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(1).AddHours(2)),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ShiftErrors.CannotReschedule);
    }

    // ---------------------------------------------------------------
    // Queries + mapping
    // ---------------------------------------------------------------

    [Fact]
    public async Task GetById_NotFound_Should_Return_NotFound()
    {
        var handler = new GetShiftByIdQueryHandler(_shifts, Mapper());

        var result = await handler.Handle(
            new GetShiftByIdQuery(Guid.NewGuid()), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Shift.NotFound");
    }

    [Fact]
    public async Task GetById_Found_Should_Flatten_Domain_ValueObjects()
    {
        var shift = await CreatedShiftAsync(HiredDriver());
        shift.Start();
        var handler = new GetShiftByIdQueryHandler(_shifts, Mapper());

        var result = await handler.Handle(
            new GetShiftByIdQuery(shift.Id.Value), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(shift.Id.Value);
        result.Value.DriverId.Should().Be(shift.DriverId.Value);
        result.Value.Start.Should().Be(shift.ShiftStart);
        result.Value.End.Should().Be(shift.ShiftEnd);
        result.Value.Status.Should().Be("Active");
        result.Value.StartedAtUtc.Should().Be(shift.StartedAtUtc);
        result.Value.EndedAtUtc.Should().BeNull();
    }

    [Fact]
    public async Task GetAll_Should_Filter_By_Driver_And_Status()
    {
        var first = HiredDriver(name: "First", licenseNumber: "111111");
        var second = HiredDriver(name: "Second", licenseNumber: "222222");
        var firstShift = await CreatedShiftAsync(first);
        await CreatedShiftAsync(second);

        var handler = new GetAllShiftsQueryHandler(_shifts, Mapper());

        var all = await handler.Handle(new GetAllShiftsQuery(), CancellationToken.None);
        var byDriver = await handler.Handle(new GetAllShiftsQuery(first.Id.Value), CancellationToken.None);
        var byStatus = await handler.Handle(
            new GetAllShiftsQuery(null, ShiftStatus.Planned), CancellationToken.None);

        all.Value.Should().HaveCount(2);
        byDriver.Value.Should().ContainSingle().Which.Id.Should().Be(firstShift.Id.Value);
        byStatus.Value.Should().HaveCount(2);
    }

    [Fact]
    public void ShiftProfile_Configuration_Should_Be_Valid()
    {
        // Regression: the old profile mapped Id only, leaving DriverId,
        // Start/End and Status to conventions that cannot see value objects.
        var configuration = new MapperConfiguration(
            cfg => cfg.AddProfile<ShiftProfile>(),
            NullLoggerFactory.Instance);

        configuration.AssertConfigurationIsValid();
    }

    // ---------------------------------------------------------------
    // ShiftClosedEventHandler
    // ---------------------------------------------------------------

    private ShiftClosedEventHandler ClosedHandler()
        => new(_drivers, _uow, NullLogger<ShiftClosedEventHandler>.Instance);

    [Fact]
    public async Task CompletedShift_Should_Release_Driver_Current_Shift()
    {
        var driver = HiredDriver();
        var shift = await CreatedShiftAsync(driver);
        var savedBefore = _uow.SaveCount;

        await ClosedHandler().Handle(
            new ShiftCompletedEvent(shift.Id, driver.Id, DateTime.UtcNow, DateTime.UtcNow),
            CancellationToken.None);

        driver.CurrentShiftId.Should().BeNull();
        _uow.SaveCount.Should().Be(savedBefore + 1);
    }

    [Fact]
    public async Task CancelledShift_Should_Release_Driver_Current_Shift()
    {
        var driver = HiredDriver();
        var shift = await CreatedShiftAsync(driver);

        await ClosedHandler().Handle(
            new ShiftCancelledEvent(shift.Id, driver.Id, shift.ShiftStart, shift.ShiftEnd),
            CancellationToken.None);

        driver.CurrentShiftId.Should().BeNull();
    }

    [Fact]
    public async Task AbortedShift_Should_Release_Driver_Current_Shift()
    {
        var driver = HiredDriver();
        var shift = await CreatedShiftAsync(driver);

        await ClosedHandler().Handle(
            new ShiftAbortedEvent(shift.Id, driver.Id, DateTime.UtcNow),
            CancellationToken.None);

        driver.CurrentShiftId.Should().BeNull();
    }

    [Fact]
    public async Task ClosedShift_Should_Not_Save_When_Driver_Is_On_Another_Shift()
    {
        var driver = HiredDriver();
        await CreatedShiftAsync(driver);
        var savedBefore = _uow.SaveCount;

        await ClosedHandler().Handle(
            new ShiftCompletedEvent(
                Domain.Shifts.ValueObjects.ShiftId.New(), driver.Id, DateTime.UtcNow, DateTime.UtcNow),
            CancellationToken.None);

        driver.CurrentShiftId.Should().NotBeNull();
        _uow.SaveCount.Should().Be(savedBefore);
    }

    [Fact]
    public async Task ClosedShift_Should_Not_Throw_When_Driver_Is_Missing()
    {
        var savedBefore = _uow.SaveCount;

        var act = async () => await ClosedHandler().Handle(
            new ShiftCompletedEvent(
                Domain.Shifts.ValueObjects.ShiftId.New(),
                DriverId.New(),
                DateTime.UtcNow,
                DateTime.UtcNow),
            CancellationToken.None);

        await act.Should().NotThrowAsync();
        _uow.SaveCount.Should().Be(savedBefore);
    }
}

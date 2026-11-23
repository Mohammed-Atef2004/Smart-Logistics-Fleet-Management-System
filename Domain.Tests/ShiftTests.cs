using System.Reflection;
using Domain.Drivers.ValueObjects;
using Domain.SharedKernel;
using Domain.Shifts;
using Domain.Shifts.Enums;
using Domain.Shifts.Errors;
using Domain.Shifts.Events;
using Domain.Shifts.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Domain.Tests;

/// <summary>
/// Domain coverage for the Shift aggregate: the full lifecycle state
/// machine, window validation, UTC normalization, the actual start/end
/// timestamps, rescheduling, and the event payload contract.
/// </summary>
public class ShiftTests
{
    // ---------------------------------------------------------------
    // Helpers
    // ---------------------------------------------------------------

    private static Shift NewShift(
        DateTime? start = null,
        DateTime? end = null,
        DriverId? driverId = null)
    {
        var shiftStart = start ?? DateTime.UtcNow.AddHours(1);
        var shiftEnd = end ?? shiftStart.AddHours(4);

        return Shift.Create(driverId ?? DriverId.New(), shiftStart, shiftEnd).Value;
    }

    private static Shift ActiveShift()
    {
        var shift = NewShift();
        shift.Start().IsSuccess.Should().BeTrue();
        return shift;
    }

    // ---------------------------------------------------------------
    // Create
    // ---------------------------------------------------------------

    [Fact]
    public void Create_Should_Create_Planned_Shift_And_Raise_Event()
    {
        var driverId = DriverId.New();
        var start = DateTime.UtcNow.AddHours(1);
        var end = start.AddHours(4);

        var result = Shift.Create(driverId, start, end);

        result.IsSuccess.Should().BeTrue();
        var shift = result.Value;
        shift.Id.IsEmpty.Should().BeFalse();
        shift.DriverId.Should().Be(driverId);
        shift.Status.Should().Be(ShiftStatus.Planned);
        shift.ShiftStart.Should().Be(start);
        shift.ShiftEnd.Should().Be(end);
        shift.Duration.Should().Be(TimeSpan.FromHours(4));
        shift.StartedAtUtc.Should().BeNull();
        shift.EndedAtUtc.Should().BeNull();

        shift.DomainEvents.OfType<ShiftCreatedEvent>()
            .Should().ContainSingle(e =>
                e.ShiftId == shift.Id &&
                e.DriverId == driverId &&
                e.ShiftStart == start &&
                e.ShiftEnd == end);
    }

    [Fact]
    public void Create_Should_Fail_When_Driver_Is_Empty()
    {
        var result = Shift.Create(default, DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(2));

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ShiftErrors.InvalidDriver);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_Should_Fail_When_Start_Is_Not_Before_End(int offsetHours)
    {
        var start = DateTime.UtcNow.AddHours(2);
        var end = start.AddHours(offsetHours);

        var result = Shift.Create(DriverId.New(), start, end);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ShiftErrors.InvalidDuration);
    }

    [Fact]
    public void Create_Should_Fail_When_Duration_Exceeds_Max()
    {
        var start = DateTime.UtcNow.AddHours(1);

        var result = Shift.Create(
            DriverId.New(), start, start.AddHours(Shift.MaxDurationHours + 1));

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ShiftErrors.DurationTooLong);
    }

    [Fact]
    public void Create_Should_Accept_Max_Duration()
    {
        var start = DateTime.UtcNow.AddHours(1);

        var result = Shift.Create(
            DriverId.New(), start, start.AddHours(Shift.MaxDurationHours));

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Create_Should_Normalize_Local_Values_To_Utc()
    {
        var localStart = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(1), DateTimeKind.Local);
        var localEnd = DateTime.SpecifyKind(localStart.AddHours(2), DateTimeKind.Local);

        var shift = Shift.Create(DriverId.New(), localStart, localEnd).Value;

        shift.ShiftStart.Kind.Should().Be(DateTimeKind.Utc);
        shift.ShiftEnd.Kind.Should().Be(DateTimeKind.Utc);
        shift.ShiftStart.Should().Be(localStart.ToUniversalTime());
    }

    [Fact]
    public void Create_Should_Treat_Unspecified_Kind_As_Utc()
    {
        var start = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(1), DateTimeKind.Unspecified);
        var end = DateTime.SpecifyKind(start.AddHours(2), DateTimeKind.Unspecified);

        var shift = Shift.Create(DriverId.New(), start, end).Value;

        shift.ShiftStart.Kind.Should().Be(DateTimeKind.Utc);
        shift.ShiftStart.Should().Be(start);
    }

    // ---------------------------------------------------------------
    // Start
    // ---------------------------------------------------------------

    [Fact]
    public void Start_Should_Activate_Shift_And_Record_Actual_Start()
    {
        var shift = NewShift();

        var result = shift.Start();

        result.IsSuccess.Should().BeTrue();
        shift.Status.Should().Be(ShiftStatus.Active);
        shift.StartedAtUtc.Should().NotBeNull();
        shift.DomainEvents.OfType<ShiftStartedEvent>()
            .Should().ContainSingle(e => e.ShiftId == shift.Id && e.DriverId == shift.DriverId);
    }

    [Fact]
    public void Start_Should_Fail_When_Not_Planned()
    {
        var shift = ActiveShift();

        // Regression: the old code only checked Planned, so this is mostly a
        // guard against future regressions in the state machine.
        var result = shift.Start();

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ShiftErrors.CannotStart);
    }

    [Fact]
    public void Start_Should_Fail_When_Window_Has_Already_Ended()
    {
        var shift = NewShift(
            DateTime.UtcNow.AddHours(-4),
            DateTime.UtcNow.AddHours(-2));

        var result = shift.Start();

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ShiftErrors.AlreadyEnded);
        shift.Status.Should().Be(ShiftStatus.Planned);
    }

    // ---------------------------------------------------------------
    // Complete
    // ---------------------------------------------------------------

    [Fact]
    public void Complete_Should_Complete_Active_Shift_And_Record_End()
    {
        var shift = ActiveShift();
        var startedAt = shift.StartedAtUtc;

        var result = shift.Complete();

        result.IsSuccess.Should().BeTrue();
        shift.Status.Should().Be(ShiftStatus.Completed);
        shift.EndedAtUtc.Should().NotBeNull();
        shift.DomainEvents.OfType<ShiftCompletedEvent>()
            .Should().ContainSingle(e =>
                e.ShiftId == shift.Id &&
                e.StartedAtUtc == startedAt &&
                e.EndedAtUtc == shift.EndedAtUtc);
    }

    [Fact]
    public void Complete_Should_Fail_When_Shift_Is_Only_Planned()
    {
        var shift = NewShift();

        var result = shift.Complete();

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ShiftErrors.CannotComplete);
    }

    [Fact]
    public void Complete_Should_Fail_When_Already_Completed()
    {
        var shift = ActiveShift();
        shift.Complete();

        var result = shift.Complete();

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ShiftErrors.CannotComplete);
        shift.DomainEvents.OfType<ShiftCompletedEvent>().Should().ContainSingle();
    }

    // ---------------------------------------------------------------
    // Cancel / Abort
    // ---------------------------------------------------------------

    [Fact]
    public void Cancel_Should_Cancel_Planned_Shift()
    {
        var shift = NewShift();

        var result = shift.Cancel();

        result.IsSuccess.Should().BeTrue();
        shift.Status.Should().Be(ShiftStatus.Cancelled);
        shift.DomainEvents.OfType<ShiftCancelledEvent>()
            .Should().ContainSingle(e => e.ShiftId == shift.Id);
    }

    [Fact]
    public void Cancel_Should_Fail_When_Shift_Is_Active()
    {
        var shift = ActiveShift();

        var result = shift.Cancel();

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ShiftErrors.CannotCancel);
        shift.Status.Should().Be(ShiftStatus.Active);
    }

    [Fact]
    public void Cancel_Should_Fail_When_Already_Cancelled()
    {
        var shift = NewShift();
        shift.Cancel();

        // Regression: cancelling twice used to succeed and raise a second event.
        var result = shift.Cancel();

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ShiftErrors.CannotCancel);
        shift.DomainEvents.OfType<ShiftCancelledEvent>().Should().ContainSingle();
    }

    [Fact]
    public void Abort_Should_Abort_Active_Shift_And_Record_End()
    {
        var shift = ActiveShift();

        var result = shift.Abort();

        result.IsSuccess.Should().BeTrue();
        shift.Status.Should().Be(ShiftStatus.Aborted);
        shift.EndedAtUtc.Should().NotBeNull();
        shift.DomainEvents.OfType<ShiftAbortedEvent>()
            .Should().ContainSingle(e => e.ShiftId == shift.Id);
    }

    [Fact]
    public void Abort_Should_Fail_When_Shift_Is_Only_Planned()
    {
        var shift = NewShift();

        var result = shift.Abort();

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ShiftErrors.CannotAbort);
    }

    // ---------------------------------------------------------------
    // Reschedule
    // ---------------------------------------------------------------

    [Fact]
    public void Reschedule_Should_Move_Planned_Shift_And_Raise_Event()
    {
        var shift = NewShift();
        var newStart = DateTime.UtcNow.AddDays(1);
        var newEnd = newStart.AddHours(6);

        var result = shift.Reschedule(newStart, newEnd);

        result.IsSuccess.Should().BeTrue();
        shift.ShiftStart.Should().Be(newStart);
        shift.ShiftEnd.Should().Be(newEnd);
        shift.DomainEvents.OfType<ShiftRescheduledEvent>()
            .Should().ContainSingle(e => e.ShiftStart == newStart && e.ShiftEnd == newEnd);
    }

    [Fact]
    public void Reschedule_Should_Be_NoOp_For_Same_Window()
    {
        var shift = NewShift();

        var result = shift.Reschedule(shift.ShiftStart, shift.ShiftEnd);

        result.IsSuccess.Should().BeTrue();
        shift.DomainEvents.OfType<ShiftRescheduledEvent>().Should().BeEmpty();
    }

    [Fact]
    public void Reschedule_Should_Fail_When_Shift_Is_Active()
    {
        var shift = ActiveShift();

        var result = shift.Reschedule(DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(1).AddHours(2));

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ShiftErrors.CannotReschedule);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Reschedule_Should_Reject_Invalid_Window(int offsetHours)
    {
        var shift = NewShift();
        var start = DateTime.UtcNow.AddDays(1);

        var result = shift.Reschedule(start, start.AddHours(offsetHours));

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ShiftErrors.InvalidDuration);
    }

    [Fact]
    public void Reschedule_Should_Reject_Window_Longer_Than_Max()
    {
        var shift = NewShift();
        var start = DateTime.UtcNow.AddDays(1);

        var result = shift.Reschedule(start, start.AddHours(Shift.MaxDurationHours + 1));

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ShiftErrors.DurationTooLong);
    }

    // ---------------------------------------------------------------
    // Overlap helper
    // ---------------------------------------------------------------

    [Fact]
    public void Overlaps_Should_Use_Half_Open_Intervals()
    {
        var start = DateTime.UtcNow.AddHours(2);
        var shift = NewShift(start, start.AddHours(2));

        shift.Overlaps(start.AddHours(-1), start).Should().BeFalse("the windows only touch");
        shift.Overlaps(start.AddHours(2), start.AddHours(3)).Should().BeFalse("the windows only touch");
        shift.Overlaps(start.AddHours(1), start.AddHours(3)).Should().BeTrue();
        shift.Overlaps(start.AddHours(-1), start.AddMinutes(1)).Should().BeTrue();
    }

    // ---------------------------------------------------------------
    // ShiftId
    // ---------------------------------------------------------------

    [Fact]
    public void ShiftId_New_Should_Produce_Unique_Non_Empty_Ids()
    {
        var first = ShiftId.New();
        var second = ShiftId.New();

        first.IsEmpty.Should().BeFalse();
        first.Should().NotBe(second);
    }

    [Fact]
    public void ShiftId_Default_Should_Be_Empty()
    {
        default(ShiftId).IsEmpty.Should().BeTrue();
    }

    // ---------------------------------------------------------------
    // Guard: event payloads must never shadow DomainEvent.Id
    // ---------------------------------------------------------------

    [Fact]
    public void Shift_Events_Must_Not_Declare_A_Property_Named_Id()
    {
        var eventTypes = typeof(Shift).Assembly.GetTypes()
            .Where(t => t.Namespace == typeof(ShiftCreatedEvent).Namespace)
            .Where(t => typeof(DomainEvent).IsAssignableFrom(t) && !t.IsAbstract);

        foreach (var eventType in eventTypes)
        {
            var properties = eventType.GetProperties(
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            Assert.DoesNotContain(properties, p => p.Name == "Id");
        }
    }
}

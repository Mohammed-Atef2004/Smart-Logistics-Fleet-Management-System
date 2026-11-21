using System.Reflection;
using Domain.Drivers;
using Domain.Drivers.Enums;
using Domain.Drivers.Errors;
using Domain.Drivers.Events;
using Domain.Drivers.ValueObjects;
using Domain.SharedKernel;
using Domain.Shifts.ValueObjects;
using Domain.Tests;
using FluentAssertions;
using Xunit;
using DriverAggregate = Domain.Drivers.Driver;

public class DriverTests
{
    // ---------------------------------------------------------------
    // Helpers
    // ---------------------------------------------------------------

    private static DriverLicense ValidLicense(string number = "123456") =>
        DriverLicense.Create(number, DateTime.UtcNow.AddYears(1), "B").Value;

    private static DriverAggregate HiredDriver(
        string name = "Mohamed Atef",
        FakeDriverUniquenessChecker? checker = null)
        => DriverAggregate.Hire(name, ValidLicense(), checker ?? new FakeDriverUniquenessChecker()).Value;

    private static ShiftId NewShiftId() => new(Guid.NewGuid());

    // ---------------------------------------------------------------
    // Hire
    // ---------------------------------------------------------------

    [Fact]
    public void Hire_Should_Create_Driver_With_Valid_Data()
    {
        var license = ValidLicense();

        var result = DriverAggregate.Hire("Mohamed Atef", license, new FakeDriverUniquenessChecker());

        result.IsSuccess.Should().BeTrue();
        result.Value.FullName.Should().Be("Mohamed Atef");
        result.Value.Status.Should().Be(DriverStatus.Active);
        result.Value.License.Should().BeEquivalentTo(license);
        result.Value.Rating.Value.Should().Be(5.0);
        result.Value.Rating.Count.Should().Be(0);
        result.Value.Id.Value.Should().NotBe(Guid.Empty);
        result.Value.DomainEvents.OfType<DriverHiredEvent>().Should().ContainSingle();
    }

    [Fact]
    public void Hire_Should_Trim_Name()
    {
        var result = DriverAggregate.Hire("  Mohamed  ", ValidLicense(), new FakeDriverUniquenessChecker());

        result.IsSuccess.Should().BeTrue();
        result.Value.FullName.Should().Be("Mohamed");
    }

    [Fact]
    public void Hire_Should_Fail_When_Name_Is_Empty()
    {
        var result = DriverAggregate.Hire("", ValidLicense(), new FakeDriverUniquenessChecker());

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DriverErrors.EmptyName);
    }

    [Fact]
    public void Hire_Should_Fail_When_Name_Is_Too_Long()
    {
        var name = new string('a', DriverAggregate.MaxNameLength + 1);

        var result = DriverAggregate.Hire(name, ValidLicense(), new FakeDriverUniquenessChecker());

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DriverErrors.NameTooLong);
    }

    [Fact]
    public void Hire_Should_Fail_When_License_Number_Already_Exists()
    {
        var checker = new FakeDriverUniquenessChecker(isUnique: false);

        var result = DriverAggregate.Hire("Mohamed", ValidLicense(), checker);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DriverErrors.LicenseNumberAlreadyExists);
    }

    // ---------------------------------------------------------------
    // Suspend / Reactivate / Terminate (state machine)
    // ---------------------------------------------------------------

    [Fact]
    public void Suspend_Should_Work_When_Active()
    {
        var driver = HiredDriver();

        var result = driver.Suspend(DriverSuspensionReason.Misconduct);

        result.IsSuccess.Should().BeTrue();
        driver.Status.Should().Be(DriverStatus.Suspended);
        driver.DomainEvents.OfType<DriverSuspendedEvent>()
            .Should().ContainSingle(e => e.Reason == DriverSuspensionReason.Misconduct);
    }

    [Fact]
    public void Suspend_Should_Fail_When_Already_Suspended()
    {
        var driver = HiredDriver();
        driver.Suspend(DriverSuspensionReason.Misconduct);

        // Regression: the old code ignored the rule result and reported success.
        var result = driver.Suspend(DriverSuspensionReason.Fraud);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DriverErrors.AlreadySuspended);
        driver.Status.Should().Be(DriverStatus.Suspended);
    }

    [Fact]
    public void Suspend_Should_Fail_When_Terminated()
    {
        var driver = HiredDriver();
        driver.Terminate();

        var result = driver.Suspend(DriverSuspensionReason.Fraud);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DriverErrors.Terminated);
        driver.Status.Should().Be(DriverStatus.Terminated);
    }

    [Fact]
    public void Reactivate_Should_Work_When_Suspended()
    {
        var driver = HiredDriver();
        driver.Suspend(DriverSuspensionReason.Misconduct);

        var result = driver.Reactivate();

        result.IsSuccess.Should().BeTrue();
        driver.Status.Should().Be(DriverStatus.Active);
        driver.DomainEvents.OfType<DriverReactivatedEvent>().Should().ContainSingle();
    }

    [Fact]
    public void Reactivate_Should_Fail_When_Active()
    {
        var driver = HiredDriver();

        // Regression: the old code allowed Active -> Active.
        var result = driver.Reactivate();

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DriverErrors.NotSuspended);
        driver.Status.Should().Be(DriverStatus.Active);
    }

    [Fact]
    public void Reactivate_Should_Fail_When_Terminated()
    {
        var driver = HiredDriver();
        driver.Terminate();

        var result = driver.Reactivate();

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DriverErrors.Terminated);
    }

    [Fact]
    public void Terminate_Should_Set_Terminated_And_Release_Shift()
    {
        var driver = HiredDriver();
        var shiftId = NewShiftId();
        driver.AssignShift(shiftId);

        var result = driver.Terminate();

        result.IsSuccess.Should().BeTrue();
        driver.Status.Should().Be(DriverStatus.Terminated);
        driver.CurrentShiftId.Should().BeNull();
        driver.DomainEvents.OfType<DriverTerminatedEvent>().Should().ContainSingle();
        driver.DomainEvents.OfType<DriverShiftClearedEvent>()
            .Should().ContainSingle(e => e.ShiftId == shiftId);
    }

    [Fact]
    public void Terminate_Should_Fail_When_Already_Terminated()
    {
        var driver = HiredDriver();
        driver.Terminate();

        var result = driver.Terminate();

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DriverErrors.AlreadyTerminated);
    }

    // ---------------------------------------------------------------
    // Shift assignment
    // ---------------------------------------------------------------

    [Fact]
    public void AssignShift_Should_Assign_When_Active_And_No_CurrentShift()
    {
        var driver = HiredDriver();
        var shiftId = NewShiftId();

        var result = driver.AssignShift(shiftId);

        result.IsSuccess.Should().BeTrue();
        driver.CurrentShiftId.Should().Be(shiftId);
        driver.DomainEvents.OfType<DriverShiftAssignedEvent>()
            .Should().ContainSingle(e => e.ShiftId == shiftId);
    }

    [Fact]
    public void AssignShift_Should_Fail_When_Already_In_Shift()
    {
        var driver = HiredDriver();
        driver.AssignShift(NewShiftId());

        var result = driver.AssignShift(NewShiftId());

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DriverErrors.AlreadyInShift);
    }

    [Fact]
    public void AssignShift_Should_Fail_When_Suspended()
    {
        var driver = HiredDriver();
        driver.Suspend(DriverSuspensionReason.Misconduct);

        var result = driver.AssignShift(NewShiftId());

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DriverErrors.NotActive);
    }

    [Fact]
    public void AssignShift_Should_Fail_When_ShiftId_Is_Null()
    {
        var driver = HiredDriver();

        var result = driver.AssignShift(null!);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DriverErrors.InvalidShift);
    }

    [Fact]
    public void ClearShift_Should_Clear_CurrentShift()
    {
        var driver = HiredDriver();
        var shiftId = NewShiftId();
        driver.AssignShift(shiftId);

        var result = driver.ClearShift();

        result.IsSuccess.Should().BeTrue();
        driver.CurrentShiftId.Should().BeNull();
        driver.DomainEvents.OfType<DriverShiftClearedEvent>()
            .Should().ContainSingle(e => e.ShiftId == shiftId);
    }

    [Fact]
    public void ClearShift_Should_Fail_When_No_Shift()
    {
        var driver = HiredDriver();

        // Regression: the old method silently succeeded when nothing was assigned.
        var result = driver.ClearShift();

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DriverErrors.NotInShift);
    }

    // ---------------------------------------------------------------
    // Ratings
    // ---------------------------------------------------------------

    [Fact]
    public void RecordTripRating_Should_Update_Rating()
    {
        var driver = HiredDriver();

        driver.RecordTripRating(4.0);

        driver.Rating.Value.Should().Be(4.0); // (5*0 + 4) / 1 = 4
        driver.Rating.Count.Should().Be(1);
    }

    [Fact]
    public void RecordTripRating_Should_Reject_NaN()
    {
        var driver = HiredDriver();

        // Regression: `rating < 1 || rating > 5` accepted NaN and poisoned
        // the running average permanently.
        var result = driver.RecordTripRating(double.NaN);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DriverErrors.InvalidRating);
        driver.Rating.Count.Should().Be(0);
    }

    [Theory]
    [InlineData(0.9)]
    [InlineData(5.1)]
    [InlineData(double.PositiveInfinity)]
    public void RecordTripRating_Should_Reject_Out_Of_Range(double rating)
    {
        var driver = HiredDriver();

        var result = driver.RecordTripRating(rating);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DriverErrors.InvalidRating);
    }

    [Fact]
    public void RecordTripRating_Should_Raise_Performance_Event_Below_Threshold()
    {
        var driver = HiredDriver();

        driver.RecordTripRating(2.0);

        driver.IsUnderperforming().Should().BeTrue();
        driver.DomainEvents.OfType<DriverPerformanceDroppedEvent>()
            .Should().ContainSingle(e => e.Rating == 2.0 && e.RatingCount == 1);
    }

    [Fact]
    public void RecordTripRating_Should_Not_Raise_Performance_Event_At_Or_Above_Threshold()
    {
        var driver = HiredDriver();

        driver.RecordTripRating(DriverAggregate.LowPerformanceThreshold);

        driver.DomainEvents.OfType<DriverPerformanceDroppedEvent>().Should().BeEmpty();
    }

    // ---------------------------------------------------------------
    // Name / license updates
    // ---------------------------------------------------------------

    [Fact]
    public void UpdateName_Should_Fail_When_Empty()
    {
        var driver = HiredDriver();

        var result = driver.UpdateName("");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DriverErrors.EmptyName);
    }

    [Fact]
    public void UpdateName_Should_Be_NoOp_When_Same_Name()
    {
        var driver = HiredDriver("Mohamed");

        var result = driver.UpdateName("Mohamed");

        result.IsSuccess.Should().BeTrue();
        driver.DomainEvents.OfType<DriverNameUpdatedEvent>().Should().BeEmpty();
    }

    [Fact]
    public void UpdateName_Should_Raise_Event_With_Old_And_New_Name()
    {
        var driver = HiredDriver("Mohamed");

        var result = driver.UpdateName("Ahmed");

        result.IsSuccess.Should().BeTrue();
        driver.FullName.Should().Be("Ahmed");
        driver.DomainEvents.OfType<DriverNameUpdatedEvent>()
            .Should().ContainSingle(e => e.OldName == "Mohamed" && e.NewName == "Ahmed");
    }

    [Fact]
    public void UpdateLicense_Should_Change_License()
    {
        var newLicense = DriverLicense.Create("999999", DateTime.UtcNow.AddYears(2), "B").Value;
        var driver = HiredDriver();

        var result = driver.UpdateLicense(newLicense, new FakeDriverUniquenessChecker());

        result.IsSuccess.Should().BeTrue();
        driver.License.Should().BeEquivalentTo(newLicense);
        driver.DomainEvents.OfType<DriverLicenseUpdatedEvent>()
            .Should().ContainSingle(e => e.LicenseNumber == "999999");
    }

    [Fact]
    public void UpdateLicense_Should_Fail_When_Number_Belongs_To_Another_Driver()
    {
        var driver = HiredDriver();
        var checker = new FakeDriverUniquenessChecker(isUnique: false);

        var result = driver.UpdateLicense(ValidLicense("999999"), checker);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DriverErrors.LicenseNumberAlreadyExists);
    }

    [Fact]
    public void UpdateLicense_Should_Exclude_The_Driver_Being_Updated()
    {
        var driver = HiredDriver();
        var checker = new FakeDriverUniquenessChecker();

        driver.UpdateLicense(ValidLicense("123456"), checker);

        checker.LastExcludedDriverId.Should().Be(driver.Id);
    }

    // ---------------------------------------------------------------
    // Value objects
    // ---------------------------------------------------------------

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void DriverLicense_Should_Reject_Empty_Number(string number)
    {
        var result = DriverLicense.Create(number, DateTime.UtcNow.AddYears(1), "B");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DriverErrors.InvalidLicenseNumber);
    }

    [Fact]
    public void DriverLicense_Should_Reject_Number_Too_Long()
    {
        var number = new string('1', DriverLicense.MaxLicenseNumberLength + 1);

        var result = DriverLicense.Create(number, DateTime.UtcNow.AddYears(1), "B");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DriverErrors.LicenseNumberTooLong);
    }

    [Fact]
    public void DriverLicense_Should_Reject_Category_Too_Long()
    {
        var category = new string('C', DriverLicense.MaxCategoryLength + 1);

        var result = DriverLicense.Create("123456", DateTime.UtcNow.AddYears(1), category);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DriverErrors.InvalidLicenseCategory);
    }

    [Fact]
    public void DriverLicense_Should_Accept_Expiry_Today()
    {
        // A license is valid for the whole expiry day; the old check compared
        // against UtcNow and rejected a same-day expiry.
        var result = DriverLicense.Create("123456", DateTime.UtcNow, "B");

        result.IsSuccess.Should().BeTrue();
        result.Value.IsExpired().Should().BeFalse();
    }

    [Fact]
    public void DriverLicense_Should_Reject_Past_Expiry()
    {
        var result = DriverLicense.Create("123456", DateTime.UtcNow.AddDays(-1), "B");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DriverErrors.LicenseExpired);
    }

    [Fact]
    public void DriverRating_Should_Average_And_Round_To_Two_Decimals()
    {
        var rating = DriverRating.CreateNew()
            .AddRating(5)
            .AddRating(4.333333);

        rating.Count.Should().Be(2);
        rating.Value.Should().Be(4.67);
    }

    [Fact]
    public void DriverId_New_Should_Produce_Unique_Non_Empty_Ids()
    {
        var first = DriverId.New();
        var second = DriverId.New();

        first.Value.Should().NotBe(Guid.Empty);
        first.Should().NotBe(second);
    }

    // ---------------------------------------------------------------
    // Guard: event payloads must never shadow DomainEvent.Id
    // ---------------------------------------------------------------

    [Fact]
    public void Driver_Events_Must_Not_Declare_A_Property_Named_Id()
    {
        var eventTypes = typeof(DriverAggregate).Assembly.GetTypes()
            .Where(t => t.Namespace == typeof(DriverHiredEvent).Namespace)
            .Where(t => typeof(DomainEvent).IsAssignableFrom(t) && !t.IsAbstract);

        foreach (var eventType in eventTypes)
        {
            var properties = eventType.GetProperties(
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            Assert.DoesNotContain(properties, p => p.Name == "Id");
        }
    }
}

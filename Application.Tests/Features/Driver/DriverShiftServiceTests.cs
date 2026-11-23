using Application.Features.Driver.DomainServices;
using Application.Tests.Fakes;
using Domain.Drivers.Enums;
using Domain.Drivers.Errors;
using Domain.Drivers.ValueObjects;
using Domain.Shifts.Errors;
using FluentAssertions;
using DriverAggregate = Domain.Drivers.Driver;
using ShiftAggregate = Domain.Shifts.Shift;

namespace Application.Tests.Features.Driver;

/// <summary>
/// Cross-aggregate Driver/Shift coordination. The old service created a
/// shift but never called <c>driver.AssignShift</c>, so the driver stayed
/// "available" and the aggregate rules were skipped.
/// </summary>
public class DriverShiftServiceTests
{
    private static (DriverShiftService service, FakeDriverRepository repo, FakeShiftRepository shifts, DriverAggregate driver)
        Build(DriverStatus status = DriverStatus.Active, bool hasOverlap = false)
    {
        var repo = new FakeDriverRepository();
        var checker = new FakeDriverUniquenessChecker(repo);
        var license = DriverLicense.Create("123456", DateTime.UtcNow.AddYears(1), "B").Value;
        var driver = DriverAggregate.Hire("Mohamed", license, checker).Value;

        if (status == DriverStatus.Suspended)
            driver.Suspend(DriverSuspensionReason.Fraud);

        repo.AddAsync(driver).Wait();

        var shifts = new FakeShiftRepository();
        if (hasOverlap)
        {
            var start = DateTime.UtcNow.AddHours(1);
            shifts.Seed(ShiftAggregate.Create(driver.Id, start, start.AddHours(2)).Value);
        }

        return (new DriverShiftService(repo, shifts), repo, shifts, driver);
    }

    [Fact]
    public async Task Assign_Should_Create_Shift_And_Set_Driver_CurrentShift()
    {
        var (service, _, shifts, driver) = Build();

        var result = await service.AssignDriverToShift(
            driver.Id, DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(5));

        result.IsSuccess.Should().BeTrue();
        shifts.Added.Should().ContainSingle();
        driver.CurrentShiftId.Should().NotBeNull();
        driver.CurrentShiftId.Should().Be(result.Value.Id);
        driver.IsAvailable().Should().BeFalse();
    }

    [Fact]
    public async Task Assign_DriverNotFound_Should_Fail()
    {
        var (service, _, shifts, _) = Build();

        var result = await service.AssignDriverToShift(
            DriverId.New(), DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(5));

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Driver.NotFound");
        shifts.Added.Should().BeEmpty();
    }

    [Fact]
    public async Task Assign_SuspendedDriver_Should_Fail()
    {
        var (service, _, shifts, driver) = Build(DriverStatus.Suspended);

        var result = await service.AssignDriverToShift(
            driver.Id, DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(5));

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DriverErrors.NotActive);
        shifts.Added.Should().BeEmpty();
    }

    [Fact]
    public async Task Assign_OverlappingShift_Should_Fail()
    {
        var (service, _, shifts, driver) = Build(hasOverlap: true);

        var result = await service.AssignDriverToShift(
            driver.Id, DateTime.UtcNow.AddHours(1), DateTime.UtcNow.AddHours(5));

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(ShiftErrors.Overlap);
        shifts.Added.Should().BeEmpty();
        driver.CurrentShiftId.Should().BeNull();
    }
}

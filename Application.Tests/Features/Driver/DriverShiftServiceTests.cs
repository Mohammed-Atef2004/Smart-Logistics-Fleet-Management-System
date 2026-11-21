using Application.Features.Driver.DomainServices;
using Application.Tests.Fakes;
using Domain.Drivers.Enums;
using Domain.Drivers.Errors;
using Domain.Drivers.ValueObjects;
using Domain.Shifts;
using Domain.Shifts.ValueObjects;
using FluentAssertions;
using ShiftAggregate = Domain.Shifts.Shift;
using DriverAggregate = Domain.Drivers.Driver;

namespace Application.Tests.Features.Driver;

/// <summary>
/// Cross-aggregate Driver/Shift coordination. The old service created a
/// shift but never called <c>driver.AssignShift</c>, so the driver stayed
/// "available" and the aggregate rules were skipped.
/// </summary>
public class DriverShiftServiceTests
{
    private sealed class FakeShiftRepository : IShiftRepository
    {
        private readonly List<ShiftAggregate> _shifts = [];
        private readonly bool _hasOverlap;

        public FakeShiftRepository(bool hasOverlap = false) => _hasOverlap = hasOverlap;

        public List<ShiftAggregate> Added { get; } = [];

        public IQueryable<ShiftAggregate> EntityQuery => _shifts.AsQueryable();

        public Task AddAsync(ShiftAggregate entity)
        {
            _shifts.Add(entity);
            Added.Add(entity);
            return Task.CompletedTask;
        }

        public void Update(ShiftAggregate entity) { }

        public Task<bool> HasActiveShift(DriverId driverId) => Task.FromResult(false);

        public Task<bool> HasOverlappingShift(DriverId driverId, DateTime start, DateTime end)
            => Task.FromResult(_hasOverlap);
    }

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

        var shifts = new FakeShiftRepository(hasOverlap);
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
        result.Error.Code.Should().Be("Shift.Overlap");
        shifts.Added.Should().BeEmpty();
        driver.CurrentShiftId.Should().BeNull();
    }
}

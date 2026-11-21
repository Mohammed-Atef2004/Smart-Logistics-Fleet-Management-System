using Application.Features.Driver.Commands.ClearShift;
using Application.Features.Driver.Commands.HireDriver;
using Application.Features.Driver.Commands.Reactivate;
using Application.Features.Driver.Commands.RecordRating;
using Application.Features.Driver.Commands.Suspend;
using Application.Features.Driver.Commands.Terminate;
using Application.Features.Driver.Commands.UpdateLicense;
using Application.Features.Driver.Commands.UpdateName;
using Application.Features.Driver.Mappings;
using Application.Features.Driver.Queries.GetById;
using Application.Tests.Fakes;
using AutoMapper;
using Domain.Drivers.Enums;
using Domain.Drivers.Errors;
using Domain.Drivers.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using DriverAggregate = Domain.Drivers.Driver;

namespace Application.Tests.Features.Driver;

/// <summary>
/// Handler-level use-case tests for the Driver module (fakes instead of
/// EF Core). Covers the golden paths plus every guard the scan added:
/// Result-returning handlers, domain-error propagation, the terminated
/// terminal state, and the mapping that used to fail at runtime.
/// </summary>
public class DriverHandlerTests
{
    private readonly FakeDriverRepository _repo = new();
    private readonly FakeUnitOfWork _uow = new();
    private readonly FakeDriverUniquenessChecker _checker;

    public DriverHandlerTests()
    {
        _checker = new FakeDriverUniquenessChecker(_repo);
    }

    private DriverAggregate HiredDriver(string name = "Mohamed Atef", string licenseNumber = "123456")
    {
        var license = DriverLicense.Create(licenseNumber, DateTime.UtcNow.AddYears(1), "B").Value;
        var driver = DriverAggregate.Hire(name, license, _checker).Value;
        _repo.AddAsync(driver).Wait();
        return driver;
    }

    private HireDriverCommandHandler HireHandler()
        => new(_uow, _repo, _checker, NullLogger<HireDriverCommandHandler>.Instance);

    private static IMapper Mapper()
        => new MapperConfiguration(
                cfg => cfg.AddProfile<DriverProfile>(),
                NullLoggerFactory.Instance)
            .CreateMapper();

    // ───────────────── HireDriverCommandHandler ─────────────────

    [Fact]
    public async Task Hire_ValidCommand_Should_AddDriver_And_Save()
    {
        var result = await HireHandler().Handle(
            new HireDriverCommand("Mohamed Atef", "123456", DateTime.UtcNow.AddYears(1), "B"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBe(Guid.Empty);
        _repo.Added.Should().ContainSingle();
        _uow.SaveCount.Should().Be(1);
    }

    [Fact]
    public async Task Hire_DuplicateLicense_Should_Fail_Without_Adding()
    {
        HiredDriver(licenseNumber: "123456");

        var result = await HireHandler().Handle(
            new HireDriverCommand("Other Driver", "123456", DateTime.UtcNow.AddYears(1), "B"),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DriverErrors.LicenseNumberAlreadyExists);
        _repo.Added.Should().HaveCount(1); // only the seeded driver
        _uow.SaveCount.Should().Be(0);
    }

    [Fact]
    public async Task Hire_InvalidLicense_Should_Return_ValueObject_Error()
    {
        // Regression: the old handler threw a raw Exception instead of
        // returning the failure.
        var tooLong = new string('1', DriverLicense.MaxLicenseNumberLength + 1);

        var result = await HireHandler().Handle(
            new HireDriverCommand("Mohamed", tooLong, DateTime.UtcNow.AddYears(1), "B"),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DriverErrors.LicenseNumberTooLong);
        _repo.Added.Should().BeEmpty();
    }

    [Fact]
    public async Task Hire_EmptyName_Should_Fail()
    {
        var result = await HireHandler().Handle(
            new HireDriverCommand("   ", "123456", DateTime.UtcNow.AddYears(1), "B"),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DriverErrors.EmptyName);
    }

    // ───────────────── Suspend / Reactivate / Terminate ─────────────────

    [Fact]
    public async Task Suspend_NotFound_Should_Return_NotFound()
    {
        var handler = new SuspendDriverCommandHandler(_uow, _repo);

        var result = await handler.Handle(
            new SuspendDriverCommand(Guid.NewGuid(), DriverSuspensionReason.Fraud),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Driver.NotFound");
    }

    [Fact]
    public async Task Suspend_AlreadySuspended_Should_Fail_Without_Saving()
    {
        var driver = HiredDriver();
        driver.Suspend(DriverSuspensionReason.Fraud);
        var handler = new SuspendDriverCommandHandler(_uow, _repo);

        var result = await handler.Handle(
            new SuspendDriverCommand(driver.Id.Value, DriverSuspensionReason.Misconduct),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DriverErrors.AlreadySuspended);
        _uow.SaveCount.Should().Be(0);
    }

    [Fact]
    public async Task Suspend_Active_Should_Save()
    {
        var driver = HiredDriver();
        var handler = new SuspendDriverCommandHandler(_uow, _repo);

        var result = await handler.Handle(
            new SuspendDriverCommand(driver.Id.Value, DriverSuspensionReason.Misconduct),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        driver.Status.Should().Be(DriverStatus.Suspended);
        _uow.SaveCount.Should().Be(1);
    }

    [Fact]
    public async Task Reactivate_Suspended_Should_Save()
    {
        var driver = HiredDriver();
        driver.Suspend(DriverSuspensionReason.Fraud);
        var handler = new ReactivateDriverCommandHandler(_uow, _repo);

        var result = await handler.Handle(
            new ReactivateDriverCommand(driver.Id.Value), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        driver.Status.Should().Be(DriverStatus.Active);
        _uow.SaveCount.Should().Be(1);
    }

    [Fact]
    public async Task Reactivate_Active_Should_Fail()
    {
        var driver = HiredDriver();
        var handler = new ReactivateDriverCommandHandler(_uow, _repo);

        var result = await handler.Handle(
            new ReactivateDriverCommand(driver.Id.Value), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DriverErrors.NotSuspended);
        _uow.SaveCount.Should().Be(0);
    }

    [Fact]
    public async Task Terminate_Should_Save_And_Mark_Terminated()
    {
        var driver = HiredDriver();
        var handler = new TerminateDriverCommandHandler(_uow, _repo);

        var result = await handler.Handle(
            new TerminateDriverCommand(driver.Id.Value), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        driver.Status.Should().Be(DriverStatus.Terminated);
        _uow.SaveCount.Should().Be(1);
    }

    [Fact]
    public async Task Terminate_Twice_Should_Fail()
    {
        var driver = HiredDriver();
        driver.Terminate();
        var handler = new TerminateDriverCommandHandler(_uow, _repo);

        var result = await handler.Handle(
            new TerminateDriverCommand(driver.Id.Value), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DriverErrors.AlreadyTerminated);
    }

    // ───────────────── Shift / rating / profile updates ─────────────────

    [Fact]
    public async Task ClearShift_When_No_Shift_Should_Fail()
    {
        var driver = HiredDriver();
        var handler = new ClearDriverShiftCommandHandler(_uow, _repo);

        var result = await handler.Handle(
            new ClearDriverShiftCommand(driver.Id.Value), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DriverErrors.NotInShift);
        _uow.SaveCount.Should().Be(0);
    }

    [Fact]
    public async Task RecordRating_OutOfRange_Should_Fail()
    {
        var driver = HiredDriver();
        var handler = new RecordDriverRatingCommandHandler(_uow, _repo);

        var result = await handler.Handle(
            new RecordDriverRatingCommand(driver.Id.Value, 7), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DriverErrors.InvalidRating);
        _uow.SaveCount.Should().Be(0);
    }

    [Fact]
    public async Task UpdateName_Empty_Should_Fail()
    {
        var driver = HiredDriver();
        var handler = new UpdateDriverNameCommandHandler(_uow, _repo);

        var result = await handler.Handle(
            new UpdateDriverNameCommand(driver.Id.Value, ""), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DriverErrors.EmptyName);
    }

    [Fact]
    public async Task UpdateLicense_DuplicateNumber_Should_Fail()
    {
        HiredDriver(licenseNumber: "123456");
        var other = HiredDriver(name: "Other", licenseNumber: "654321");
        var handler = new UpdateDriverLicenseCommandHandler(_uow, _repo, _checker);

        var result = await handler.Handle(
            new UpdateDriverLicenseCommand(
                other.Id.Value, "123456", DateTime.UtcNow.AddYears(1), "B"),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(DriverErrors.LicenseNumberAlreadyExists);
    }

    // ───────────────── Queries + mapping ─────────────────

    [Fact]
    public async Task GetById_NotFound_Should_Return_NotFound()
    {
        var handler = new GetDriverByIdQueryHandler(_repo, Mapper());

        var result = await handler.Handle(
            new GetDriverByIdQuery(Guid.NewGuid()), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Driver.NotFound");
    }

    [Fact]
    public async Task GetById_Found_Should_Flatten_Domain_ValueObjects()
    {
        var driver = HiredDriver();
        driver.RecordTripRating(4);
        var handler = new GetDriverByIdQueryHandler(_repo, Mapper());

        var result = await handler.Handle(
            new GetDriverByIdQuery(driver.Id.Value), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(driver.Id.Value);
        result.Value.FullName.Should().Be("Mohamed Atef");
        result.Value.Status.Should().Be("Active");
        result.Value.Rating.Should().Be(4);
        result.Value.RatingCount.Should().Be(1);
        result.Value.LicenseNumber.Should().Be("123456");
        result.Value.LicenseCategory.Should().Be("B");
        result.Value.CurrentShiftId.Should().BeNull();
    }

    [Fact]
    public void DriverProfile_Configuration_Should_Be_Valid()
    {
        // Regression: the old profile used convention mapping only, which
        // cannot map DriverRating -> double; GET by id threw at runtime.
        var configuration = new MapperConfiguration(
            cfg => cfg.AddProfile<DriverProfile>(),
            NullLoggerFactory.Instance);

        configuration.AssertConfigurationIsValid();
    }
}

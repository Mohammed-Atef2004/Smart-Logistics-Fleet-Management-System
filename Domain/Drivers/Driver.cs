using Domain.SharedKernel;
using Domain.Drivers.Enums;
using Domain.Drivers.Errors;
using Domain.Drivers.Events;
using Domain.Drivers.Rules;
using Domain.Drivers.ValueObjects;

namespace Domain.Drivers;
public sealed class Driver : AggregateRoot<DriverId>
{
    public string FullName { get; private set; }
    public DriverLicense License { get; private set; }
    public DriverStatus Status { get; private set; }
    public DriverRating Rating { get; private set; }
    public ShiftId? CurrentShiftId { get; private set; }

    private Driver() { } // EF Core

    private Driver(DriverId id, string name, DriverLicense license) : base(id)
    {
        FullName = name;
        License = license;
        Status = DriverStatus.Active;
        Rating = DriverRating.CreateNew();
    }

    public static Result<Driver> Hire(string name, DriverLicense license)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<Driver>.Failure(DriverErrors.EmptyName);

        var driver = new Driver(new DriverId(Guid.NewGuid()), name, license);
        driver.AddDomainEvent(new DriverHiredEvent(driver.Id, name));

        return Result<Driver>.Success(driver);
    }

    public Result Suspend(DriverSuspensionReason reason)
    {
        CheckRule(new DriverMustNotBeSuspendedRule(Status));

        Status = DriverStatus.Suspended;
        AddDomainEvent(new DriverSuspendedEvent(Id, reason));

        return Result.Success();
    }

    public Result Reactivate()
    {
        CheckRule(new DriverMustBeSuspendedRule(Status));

        Status = DriverStatus.Active;
        AddDomainEvent(new DriverReactivatedEvent(Id));

        return Result.Success();
    }


    public Result AssignShift(ShiftId shiftId)
    {
        CheckRule(new DriverMustBeActiveRule(Status));
        CheckRule(new LicenseMustBeValidRule(License));

        if (CurrentShiftId != null)
            return Result.Failure(DriverErrors.AlreadyInShift);

        CurrentShiftId = shiftId;
        AddDomainEvent(new DriverShiftAssignedEvent(Id, shiftId));

        return Result.Success();
    }

    public void ClearShift()
    {
        CurrentShiftId = null;
        AddDomainEvent(new DriverShiftClearedEvent(Id));
    }

    public Result RecordTripRating(double rating)
    {
        if (rating is < 1 or > 5)
            return Result.Failure(DriverErrors.InvalidRating);

        Rating = Rating.AddRating(rating);

        if (Rating.Value < 3.5)
            AddDomainEvent(new DriverPerformanceDroppedEvent(Id, Rating.Value));

        return Result.Success();
    }

    public Result UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure(DriverErrors.EmptyName);

        FullName = name;
        return Result.Success();
    }

    public Result UpdateLicense(DriverLicense license)
    {
        License = license;
        AddDomainEvent(new DriverLicenseUpdatedEvent(Id));
        return Result.Success();
    }

    public bool IsAvailable() =>
        Status == DriverStatus.Active && CurrentShiftId == null;

    public bool IsUnderperforming() =>
        Rating.Value < 3.5;
}
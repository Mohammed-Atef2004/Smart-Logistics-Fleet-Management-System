using Domain.SharedKernel;
using Domain.Drivers.Enums;
using Domain.Drivers.Errors;
using Domain.Drivers.Events;
using Domain.Drivers.Rules;
using Domain.Drivers.ValueObjects;

public sealed class Driver : AggregateRoot<DriverId>
{
    public string FullName { get; private set; }
    public DriverLicense License { get; private set; }
    public DriverStatus Status { get; private set; }
    public DriverRating Rating { get; private set; }
    public ShiftId? CurrentShiftId { get; private set; }

    private Driver() { }

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

    public void Reactivate()
    {
        Status = DriverStatus.Active;
        AddDomainEvent(new DriverReactivatedEvent(Id));
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
    }

    public Result RecordTripRating(double rating)
    {
        Rating = Rating.AddRating(rating);

        if (Rating.Value < 3.5)
            AddDomainEvent(new DriverPerformanceDroppedEvent(Id, Rating.Value));

        return Result.Success();
    }
}
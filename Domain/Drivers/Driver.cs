using Domain.Drivers.Enums;
using Domain.Drivers.Errors;
using Domain.Drivers.Events;
using Domain.Drivers.Rules;
using Domain.Drivers.ValueObjects;
using Domain.SharedKernel;
using Domain.Shifts.ValueObjects;

namespace Domain.Drivers;

/// <summary>
/// Driver aggregate root. Every state change goes through a method that
/// checks the lifecycle invariants first (Active → Suspended → Active,
/// and any state → Terminated, which is terminal). A driver may hold at
/// most one shift at a time, referenced by <see cref="ShiftId"/> only.
/// </summary>
public sealed class Driver : AggregateRoot<DriverId>
{
    public const int MaxNameLength = 100;

    /// <summary>Ratings below this average raise a performance warning.</summary>
    public const double LowPerformanceThreshold = 3.5;

    public string FullName { get; private set; } = default!;
    public DriverLicense License { get; private set; } = default!;
    public DriverStatus Status { get; private set; }
    public DriverRating Rating { get; private set; } = default!;
    public ShiftId? CurrentShiftId { get; private set; }

    private Driver() { } // EF Core

    private Driver(DriverId id, string name, DriverLicense license) : base(id)
    {
        FullName = name;
        License = license;
        Status = DriverStatus.Active;
        Rating = DriverRating.CreateNew();
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Hires a new driver. A license number identifies a person, so it
    /// must be unique across the fleet — checked by the domain service
    /// before the aggregate is created.
    /// </summary>
    public static Result<Driver> Hire(
        string name,
        DriverLicense license,
        IDriverUniquenessChecker uniquenessChecker)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<Driver>.Failure(DriverErrors.EmptyName);

        name = name.Trim();
        if (name.Length > MaxNameLength)
            return Result<Driver>.Failure(DriverErrors.NameTooLong);

        if (license is null)
            return Result<Driver>.Failure(DriverErrors.InvalidLicenseNumber);

        var licenseRule = new LicenseMustBeValidRule(license);
        if (licenseRule.IsBroken())
            return Result<Driver>.Failure(licenseRule.Error);

        if (!uniquenessChecker.IsLicenseNumberUnique(license))
            return Result<Driver>.Failure(DriverErrors.LicenseNumberAlreadyExists);

        var driver = new Driver(DriverId.New(), name, license);
        driver.AddDomainEvent(new DriverHiredEvent(driver.Id, name));

        return Result<Driver>.Success(driver);
    }

    /// <summary>Suspends an active driver. Terminated drivers are rejected.</summary>
    public Result Suspend(DriverSuspensionReason reason)
    {
        var terminated = CheckRule(new DriverMustNotBeTerminatedRule(Status));
        if (terminated.IsFailure)
            return terminated;

        var alreadySuspended = CheckRule(new DriverMustNotBeSuspendedRule(Status));
        if (alreadySuspended.IsFailure)
            return alreadySuspended;

        Status = DriverStatus.Suspended;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new DriverSuspendedEvent(Id, reason));
        return Result.Success();
    }

    /// <summary>Reactivates a suspended driver. Only Suspended → Active is legal.</summary>
    public Result Reactivate()
    {
        var terminated = CheckRule(new DriverMustNotBeTerminatedRule(Status));
        if (terminated.IsFailure)
            return terminated;

        var notSuspended = CheckRule(new DriverMustBeSuspendedRule(Status));
        if (notSuspended.IsFailure)
            return notSuspended;

        Status = DriverStatus.Active;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new DriverReactivatedEvent(Id));
        return Result.Success();
    }

    /// <summary>
    /// Terminates the driver's employment. Terminal state: it also
    /// releases the current shift, so a driver who left can never keep
    /// a shift reserved.
    /// </summary>
    public Result Terminate()
    {
        if (Status == DriverStatus.Terminated)
            return Result.Failure(DriverErrors.AlreadyTerminated);

        var previousShift = CurrentShiftId;

        Status = DriverStatus.Terminated;
        CurrentShiftId = null;
        UpdatedAt = DateTime.UtcNow;

        if (previousShift is not null)
            AddDomainEvent(new DriverShiftClearedEvent(Id, previousShift));

        AddDomainEvent(new DriverTerminatedEvent(Id));
        return Result.Success();
    }

    /// <summary>
    /// Assigns a shift to an active, licensed driver who is not already
    /// on one. Returns the domain error instead of silently ignoring it
    /// (the old code ignored the rule results entirely).
    /// </summary>
    public Result AssignShift(ShiftId shiftId)
    {
        if (shiftId is null)
            return Result.Failure(DriverErrors.InvalidShift);

        var active = CheckRule(new DriverMustBeActiveRule(Status));
        if (active.IsFailure)
            return active;

        var validLicense = CheckRule(new LicenseMustBeValidRule(License));
        if (validLicense.IsFailure)
            return validLicense;

        if (CurrentShiftId is not null)
            return Result.Failure(DriverErrors.AlreadyInShift);

        CurrentShiftId = shiftId;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new DriverShiftAssignedEvent(Id, shiftId));
        return Result.Success();
    }

    /// <summary>
    /// Releases the current shift. Fails when there is nothing to clear,
    /// so the caller cannot report a successful operation that did nothing.
    /// </summary>
    public Result ClearShift()
    {
        if (CurrentShiftId is null)
            return Result.Failure(DriverErrors.NotInShift);

        var clearedShift = CurrentShiftId;
        CurrentShiftId = null;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new DriverShiftClearedEvent(Id, clearedShift));
        return Result.Success();
    }

    /// <summary>Records a trip rating and updates the running average.</summary>
    public Result RecordTripRating(double rating)
    {
        var ratingRule = new RatingMustBeValidRule(rating);
        if (ratingRule.IsBroken())
            return Result.Failure(ratingRule.Error);

        Rating = Rating.AddRating(rating);
        UpdatedAt = DateTime.UtcNow;

        if (Rating.Value < LowPerformanceThreshold)
            AddDomainEvent(new DriverPerformanceDroppedEvent(Id, Rating.Value, Rating.Count));

        return Result.Success();
    }

    /// <summary>Renames the driver. Updating to the same name is a no-op.</summary>
    public Result UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure(DriverErrors.EmptyName);

        name = name.Trim();
        if (name.Length > MaxNameLength)
            return Result.Failure(DriverErrors.NameTooLong);

        if (string.Equals(FullName, name, StringComparison.Ordinal))
            return Result.Success();

        var oldName = FullName;
        FullName = name;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new DriverNameUpdatedEvent(Id, oldName, name));
        return Result.Success();
    }

    /// <summary>
    /// Replaces the driver's license. The new license must be valid and
    /// its number must not belong to another driver (the driver's own
    /// current number is excluded, so re-submitting it is safe).
    /// </summary>
    public Result UpdateLicense(DriverLicense license, IDriverUniquenessChecker uniquenessChecker)
    {
        if (license is null)
            return Result.Failure(DriverErrors.InvalidLicenseNumber);

        var licenseRule = new LicenseMustBeValidRule(license);
        if (licenseRule.IsBroken())
            return Result.Failure(licenseRule.Error);

        if (!uniquenessChecker.IsLicenseNumberUnique(license, Id))
            return Result.Failure(DriverErrors.LicenseNumberAlreadyExists);

        if (License == license)
            return Result.Success();

        License = license;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new DriverLicenseUpdatedEvent(Id, license.LicenseNumber, license.ExpiryDate));
        return Result.Success();
    }

    public bool IsAvailable() =>
        Status == DriverStatus.Active && CurrentShiftId is null;

    public bool IsUnderperforming() =>
        Rating.Value < LowPerformanceThreshold;
}

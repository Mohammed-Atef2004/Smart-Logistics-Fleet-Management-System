using Domain.Users;
using Domain.Users.Errors;
using Domain.Users.Events;
using Domain.Users.Rules;
using Domain.Users.ValueObjects;
using Domain.SharedKernel;
using FluentAssertions;
using Xunit;

namespace Domain.Tests.Users;

/// <summary>
/// Tests for the User aggregate covering the authentication use cases:
/// registration, login state transitions, lockout, 2FA and password history.
/// </summary>
public class UserTests
{
    private static User CreateUser(
        string identityId = "identity-1",
        UserRole role = UserRole.Student,
        string email = "user@example.com",
        string username = "username")
    {
        return User.Create(
            identityId,
            Email.Create(email).Value,
            Username.Create(username).Value,
            "John",
            "Doe",
            isEmailTaken: false,
            isUsernameTaken: false,
            role).Value;
    }

    // ───────────────────────── Registration ─────────────────────────

    [Fact]
    public void Create_ShouldSucceed_AndRaiseRegisteredEvent()
    {
        var result = User.Create(
            "identity-1",
            Email.Create("user@example.com").Value,
            Username.Create("username").Value,
            "John",
            "Doe",
            isEmailTaken: false,
            isUsernameTaken: false);

        result.IsSuccess.Should().BeTrue();
        result.Value.IsEmailConfirmed.Should().BeFalse();
        result.Value.IsActive.Should().BeTrue();
        result.Value.Role.Should().Be(UserRole.Student);
        result.Value.DomainEvents.Should().Contain(e => e is UserRegisteredDomainEvent);
    }

    [Fact]
    public void Create_WithEmptyIdentityId_ShouldFail()
    {
        var result = User.Create(
            "",
            Email.Create("user@example.com").Value,
            Username.Create("username").Value,
            "John",
            "Doe",
            isEmailTaken: false,
            isUsernameTaken: false);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.InvalidIdentityReference);
    }

    [Fact]
    public void Create_WithTakenEmail_ShouldFail()
    {
        var result = User.Create(
            "identity-1",
            Email.Create("user@example.com").Value,
            Username.Create("username").Value,
            "John",
            "Doe",
            isEmailTaken: true,
            isUsernameTaken: false);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_WithTakenUsername_ShouldFail()
    {
        var result = User.Create(
            "identity-1",
            Email.Create("user@example.com").Value,
            Username.Create("username").Value,
            "John",
            "Doe",
            isEmailTaken: false,
            isUsernameTaken: true);

        result.IsFailure.Should().BeTrue();
    }

    // ───────────────────────── Login state ─────────────────────────

    [Fact]
    public void RecordSuccessfulLogin_ShouldResetCounters_AndSetLastLogin_AndRaiseEvent()
    {
        var user = CreateUser();
        user.RecordFailedLogin("1.1.1.1");
        user.RecordFailedLogin("1.1.1.1");

        var result = user.RecordSuccessfulLogin("1.1.1.1");

        result.IsSuccess.Should().BeTrue();
        user.FailedLoginAttempts.Should().Be(0);
        user.LockedUntil.Should().BeNull();
        user.LastLoginAt.Should().NotBeNull();
        user.DomainEvents.Should().Contain(e => e is UserLoggedInSuccessfullyDomainEvent);
    }

    [Fact]
    public void RecordSuccessfulLogin_WhenDeactivated_ShouldFail()
    {
        var user = CreateUser();
        user.Deactivate("test");

        var result = user.RecordSuccessfulLogin("1.1.1.1");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.Deactivated);
    }

    [Fact]
    public void RecordFailedLogin_ShouldIncrementCounter()
    {
        var user = CreateUser();

        user.RecordFailedLogin("1.1.1.1");

        user.FailedLoginAttempts.Should().Be(1);
        user.DomainEvents.Should().Contain(e => e is UserLoginAttemptFailedDomainEvent);
    }

    [Fact]
    public void RecordFailedLogin_AtThreshold_ShouldLockAccount()
    {
        var user = CreateUser();

        for (var i = 0; i < AccountLockoutRule.MaxFailedAttempts; i++)
            user.RecordFailedLogin("1.1.1.1");

        user.LockedUntil.Should().NotBeNull();
        user.CheckAvailability().Error.Should().Be(UserErrors.SecurityErrors.AccountLocked);
        user.DomainEvents.Should().Contain(e => e is UserAccountLockedDomainEvent);
    }

    [Fact]
    public void CheckAvailability_WhenLocked_ShouldFail()
    {
        var user = CreateUser();
        for (var i = 0; i < AccountLockoutRule.MaxFailedAttempts; i++)
            user.RecordFailedLogin("1.1.1.1");

        var availability = user.CheckAvailability();

        availability.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void RecordSuccessfulLogin_AfterLockout_ShouldClearLock()
    {
        var user = CreateUser();
        for (var i = 0; i < AccountLockoutRule.MaxFailedAttempts; i++)
            user.RecordFailedLogin("1.1.1.1");

        // Simulate the lock expiry by unlocking (admin) then logging in.
        user.UnlockAccount();
        var result = user.RecordSuccessfulLogin("1.1.1.1");

        result.IsSuccess.Should().BeTrue();
        user.LockedUntil.Should().BeNull();
        user.FailedLoginAttempts.Should().Be(0);
    }

    [Fact]
    public void UnlockAccount_ShouldResetCounters_AndRaiseEvent()
    {
        var user = CreateUser();
        for (var i = 0; i < AccountLockoutRule.MaxFailedAttempts; i++)
            user.RecordFailedLogin("1.1.1.1");

        var result = user.UnlockAccount();

        result.IsSuccess.Should().BeTrue();
        user.FailedLoginAttempts.Should().Be(0);
        user.LockedUntil.Should().BeNull();
        user.DomainEvents.Should().Contain(e => e is UserAccountUnlockedDomainEvent);
    }

    // ───────────────────────── 2FA ─────────────────────────

    [Fact]
    public void EnableTwoFactor_ShouldSetState_AndRaiseEvent()
    {
        var user = CreateUser();

        var result = user.EnableTwoFactor("SECRET32BASE");

        result.IsSuccess.Should().BeTrue();
        user.IsTwoFactorEnabled.Should().BeTrue();
        user.TwoFactorSecret.Should().Be("SECRET32BASE");
        user.DomainEvents.Should().Contain(e => e is UserTwoFactorEnabledDomainEvent);
    }

    [Fact]
    public void EnableTwoFactor_WhenAlreadyEnabled_ShouldFail()
    {
        var user = CreateUser();
        user.EnableTwoFactor("SECRET32BASE");

        var result = user.EnableTwoFactor("OTHERSECRET");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.SecurityErrors.TwoFactorAlreadyEnabled);
    }

    [Fact]
    public void EnableTwoFactor_WithEmptySecret_ShouldFail()
    {
        var user = CreateUser();

        var result = user.EnableTwoFactor("");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.SecurityErrors.InvalidTwoFactorSecret);
        user.IsTwoFactorEnabled.Should().BeFalse();
    }

    [Fact]
    public void DisableTwoFactor_ShouldClearState_AndRaiseEvent()
    {
        var user = CreateUser();
        user.EnableTwoFactor("SECRET32BASE");

        var result = user.DisableTwoFactor();

        result.IsSuccess.Should().BeTrue();
        user.IsTwoFactorEnabled.Should().BeFalse();
        user.TwoFactorSecret.Should().BeNull();
        user.DomainEvents.Should().Contain(e => e is UserTwoFactorDisabledDomainEvent);
    }

    [Fact]
    public void DisableTwoFactor_WhenNotEnabled_ShouldFail()
    {
        var user = CreateUser();

        var result = user.DisableTwoFactor();

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.SecurityErrors.TwoFactorNotEnabled);
    }

    // ───────────────────────── Password history ─────────────────────────

    [Fact]
    public void RecordPasswordChange_ShouldAddToHistory_AndRaiseEvent()
    {
        var user = CreateUser();

        var result = user.RecordPasswordChange("hash-1", isPasswordReused: false);

        result.IsSuccess.Should().BeTrue();
        user.PasswordHashes.Should().ContainSingle(h => h == "hash-1");
        user.PasswordChangedAt.Should().NotBeNull();
        user.DomainEvents.Should().Contain(e => e is UserPasswordChangedDomainEvent);
    }

    [Fact]
    public void RecordPasswordChange_WhenReused_ShouldFail()
    {
        var user = CreateUser();

        var result = user.RecordPasswordChange("hash-1", isPasswordReused: true);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.SecurityErrors.PasswordAlreadyUsed);
        user.PasswordHashes.Should().BeEmpty();
    }

    [Fact]
    public void RecordPasswordChange_ShouldKeepOnlyLastHistoryDepthHashes()
    {
        var user = CreateUser();

        for (var i = 1; i <= 8; i++)
            user.RecordPasswordChange($"hash-{i}", isPasswordReused: false);

        user.PasswordHashes.Count.Should().Be(PasswordMustNotBeReusedRule.HistoryDepth);
        user.PasswordHashes.Last().Should().Be("hash-8");
        user.PasswordHashes.First().Should().Be("hash-4");
    }

    // ───────────────────────── Email confirmation ─────────────────────────

    [Fact]
    public void ConfirmEmail_ShouldConfirm_AndRaiseEvent()
    {
        var user = CreateUser();

        var result = user.ConfirmEmail();

        result.IsSuccess.Should().BeTrue();
        user.IsEmailConfirmed.Should().BeTrue();
        user.DomainEvents.Should().Contain(e => e is UserEmailConfirmedDomainEvent);
    }

    [Fact]
    public void ConfirmEmail_WhenAlreadyConfirmed_IsIdempotent()
    {
        var user = CreateUser();
        user.ConfirmEmail();
        var eventCount = user.DomainEvents.Count(e => e is UserEmailConfirmedDomainEvent);

        var result = user.ConfirmEmail();

        result.IsSuccess.Should().BeTrue();
        user.DomainEvents.Count(e => e is UserEmailConfirmedDomainEvent).Should().Be(eventCount);
    }

    // ───────────────────────── Role ─────────────────────────

    [Fact]
    public void ChangeRole_ShouldSucceed_AndRaiseEvent()
    {
        var user = CreateUser();

        var result = user.ChangeRole(UserRole.Admin);

        result.IsSuccess.Should().BeTrue();
        user.Role.Should().Be(UserRole.Admin);
        user.DomainEvents.Should().Contain(e => e is UserRoleChangedDomainEvent);
    }

    [Fact]
    public void ChangeRole_ToSameRole_ShouldFail()
    {
        var user = CreateUser(role: UserRole.Admin);

        var result = user.ChangeRole(UserRole.Admin);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.RoleAlreadyAssigned);
    }

    [Fact]
    public void ChangeRole_ToSuperAdmin_ShouldFail()
    {
        var user = CreateUser();

        var result = user.ChangeRole(UserRole.SuperAdmin);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.CannotAssignSuperAdminRole);
    }

    // ───────────────────────── Lifecycle ─────────────────────────

    [Fact]
    public void Deactivate_WhenActive_ShouldDeactivate()
    {
        var user = CreateUser();

        var result = user.Deactivate("policy violation");

        result.IsSuccess.Should().BeTrue();
        user.IsActive.Should().BeFalse();
        user.DomainEvents.Should().Contain(e => e is UserDeactivatedDomainEvent);
    }

    [Fact]
    public void Reactivate_WhenDeactivated_ShouldReactivate()
    {
        var user = CreateUser();
        user.Deactivate("test");

        var result = user.Reactivate();

        result.IsSuccess.Should().BeTrue();
        user.IsActive.Should().BeTrue();
        user.DomainEvents.Should().Contain(e => e is UserReactivatedDomainEvent);
    }

    [Fact]
    public void Delete_ShouldSoftDelete_AndBlockLogin()
    {
        var user = CreateUser();

        var result = user.Delete("account removal");

        result.IsSuccess.Should().BeTrue();
        user.IsDeleted.Should().BeTrue();
        user.IsActive.Should().BeFalse();
        user.DeletedAt.Should().NotBeNull();
        user.CheckAvailability().Error.Should().Be(UserErrors.AccountDeleted);
    }

    [Fact]
    public void Delete_WhenSuperAdmin_ShouldFail()
    {
        var user = CreateUser(role: UserRole.SuperAdmin);

        var result = user.Delete("test");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.CannotDeleteSuperAdmin);
    }
}

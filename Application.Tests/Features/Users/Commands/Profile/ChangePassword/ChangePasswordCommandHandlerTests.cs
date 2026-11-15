using Application.Features.Users.Commands.Profile.ChangePassword;
using Application.Tests.Fakes;
using Domain.Users;
using Domain.Users.Errors;
using Domain.Users.ValueObjects;
using FluentAssertions;

namespace Application.Tests.Features.Users.Commands.Profile.ChangePassword;

/// <summary>
/// Use-case tests for ChangePasswordCommandHandler: wrong current
/// password, reuse prevention, and the password history being updated.
/// </summary>
public class ChangePasswordCommandHandlerTests
{
    private const string CurrentPassword = "Password1!";
    private const string NewPassword = "NewPassword1!";

    private readonly FakeUserRepository _repo = new();
    private readonly FakeUnitOfWork _uow = new();
    private readonly FakeIdentityService _identity = new();
    private readonly FakeTokenService _tokens = new();
    private readonly FakePasswordHasher _hasher = new();
    private readonly FakeTotpService _totp = new();
    private readonly FakeAuditService _audit = new();
    private readonly TestHttpContextAccessor _http = new();
    private readonly ChangePasswordCommandHandler _handler;
    private readonly User _user;

    public ChangePasswordCommandHandlerTests()
    {
        _handler = new ChangePasswordCommandHandler(
            _repo, _identity, _tokens, _hasher, _totp, _uow, _audit, _http);

        _user = User.Create(
            "identity-1",
            Email.Create("user@example.com").Value,
            Username.Create("username").Value,
            "John", "Doe", false, false).Value;
        _user.ConfirmEmail();
        _identity.Seed("identity-1", "user@example.com", CurrentPassword);
    }

    private ChangePasswordCommand Command(string current = CurrentPassword, string @new = NewPassword, string? twoFactorCode = null)
        => new(_user.Id, current, @new, twoFactorCode);

    [Fact]
    public async Task Handle_WrongCurrentPassword_ShouldFail_AndRecordFailedLogin()
    {
        await _repo.AddAsync(_user);

        var result = await _handler.Handle(Command(current: "Wrong1!Password"), CancellationToken.None);

        result.Error.Should().Be(UserErrors.InvalidCredentials);
        _user.FailedLoginAttempts.Should().Be(1);
        _tokens.RevokedUsers.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_ValidChange_ShouldUpdateHistory_AndRevokeRefreshToken()
    {
        _user.RecordPasswordChange(_hasher.HashPassword(CurrentPassword), isPasswordReused: false);
        await _repo.AddAsync(_user);

        var result = await _handler.Handle(Command(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _user.PasswordHashes.Should().Contain(_hasher.HashPassword(NewPassword));
        _user.PasswordChangedAt.Should().NotBeNull();
        _tokens.RevokedUsers.Should().Contain("identity-1");
    }

    [Fact]
    public async Task Handle_ReusedPassword_ShouldFail_WithoutChangingIdentity()
    {
        var reusedHash = _hasher.HashPassword(NewPassword);
        _user.RecordPasswordChange(reusedHash, isPasswordReused: false);
        await _repo.AddAsync(_user);

        var result = await _handler.Handle(Command(@new: NewPassword), CancellationToken.None);

        result.Error.Should().Be(UserErrors.SecurityErrors.PasswordAlreadyUsed);
        _user.PasswordHashes.Count.Should().Be(1);
        _tokens.RevokedUsers.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_TwoFactorEnabled_WithInvalidCode_ShouldFail()
    {
        _user.EnableTwoFactor("SECRET32BASE");
        await _repo.AddAsync(_user);

        var result = await _handler.Handle(Command(), CancellationToken.None);

        result.Error.Should().Be(UserErrors.SecurityErrors.InvalidTotpCode);
        _tokens.RevokedUsers.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_TwoFactorEnabled_WithValidCode_ShouldSucceed()
    {
        _user.EnableTwoFactor("SECRET32BASE");
        _totp.SetValid("SECRET32BASE", "654321");
        await _repo.AddAsync(_user);

        var result = await _handler.Handle(Command(twoFactorCode: "654321"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _tokens.RevokedUsers.Should().Contain("identity-1");
    }
}

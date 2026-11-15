using Application.Features.Users.Commands.Authentication.VerifyTwoFactorLogin;
using Application.Tests.Fakes;
using Domain.Users;
using Domain.Users.Errors;
using Domain.Users.ValueObjects;
using FluentAssertions;

namespace Application.Tests.Features.Users.Commands.Authentication.VerifyTwoFactorLogin;

/// <summary>
/// Use-case tests for VerifyTwoFactorLoginCommandHandler: the 2FA-only
/// login step must check account availability and issue BOTH tokens.
/// </summary>
public class VerifyTwoFactorLoginCommandHandlerTests
{
    private const string Secret = "SECRET32BASE";
    private const string ValidCode = "123456";

    private readonly FakeUserRepository _repo = new();
    private readonly FakeUnitOfWork _uow = new();
    private readonly FakeIdentityService _identity = new();
    private readonly FakeTokenService _tokens = new();
    private readonly FakeTotpService _totp = new();
    private readonly FakeAuditService _audit = new();
    private readonly VerifyTwoFactorLoginCommandHandler _handler;

    public VerifyTwoFactorLoginCommandHandlerTests()
    {
        _handler = new VerifyTwoFactorLoginCommandHandler(
            _repo, _uow, _totp, _tokens, _identity, new TestHttpContextAccessor(), _audit);
    }

    private User CreateUser(bool twoFactor = true)
    {
        var user = User.Create(
            "identity-1",
            Email.Create("user@example.com").Value,
            Username.Create("username").Value,
            "John", "Doe", false, false).Value;

        user.ConfirmEmail();
        if (twoFactor)
            user.EnableTwoFactor(Secret);

        return user;
    }

    [Fact]
    public async Task Handle_ValidTotp_ShouldReturnBothTokens_AndRecordLogin()
    {
        var user = CreateUser();
        _totp.SetValid(Secret, ValidCode);
        await _repo.AddAsync(user);

        var result = await _handler.Handle(new VerifyTwoFactorLoginCommand(user.Id, ValidCode), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Token.Should().NotBeNull();
        result.Value.RefreshToken.Should().NotBeNull();
        _identity.UpdatedRefreshTokens.Should().ContainSingle(t => t.IdentityId == "identity-1");
        user.LastLoginAt.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_InvalidTotp_ShouldFail_AndRecordFailedLogin()
    {
        var user = CreateUser();
        await _repo.AddAsync(user);

        var result = await _handler.Handle(new VerifyTwoFactorLoginCommand(user.Id, "000000"), CancellationToken.None);

        result.Error.Should().Be(UserErrors.SecurityErrors.InvalidTotpCode);
        user.FailedLoginAttempts.Should().Be(1);
    }

    [Fact]
    public async Task Handle_DeactivatedUser_ShouldRefuse()
    {
        var user = CreateUser();
        user.Deactivate("policy");
        _totp.SetValid(Secret, ValidCode);
        await _repo.AddAsync(user);

        var result = await _handler.Handle(new VerifyTwoFactorLoginCommand(user.Id, ValidCode), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.Deactivated);
        user.LastLoginAt.Should().BeNull();
    }

    [Fact]
    public async Task Handle_LockedAccount_ShouldRefuse()
    {
        var user = CreateUser();
        for (var i = 0; i < 5; i++)
            user.RecordFailedLogin("1.1.1.1");
        _totp.SetValid(Secret, ValidCode);
        await _repo.AddAsync(user);

        var result = await _handler.Handle(new VerifyTwoFactorLoginCommand(user.Id, ValidCode), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.SecurityErrors.AccountLocked);
    }

    [Fact]
    public async Task Handle_WhenTwoFactorNotEnabled_ShouldRefuse()
    {
        var user = CreateUser(twoFactor: false);
        _totp.SetValid(Secret, ValidCode);
        await _repo.AddAsync(user);

        var result = await _handler.Handle(new VerifyTwoFactorLoginCommand(user.Id, ValidCode), CancellationToken.None);

        result.Error.Should().Be(UserErrors.SecurityErrors.TwoFactorNotEnabled);
    }

    [Fact]
    public async Task Handle_FiveInvalidCodes_ShouldLockAccount()
    {
        var user = CreateUser();
        await _repo.AddAsync(user);

        for (var i = 0; i < 5; i++)
            await _handler.Handle(new VerifyTwoFactorLoginCommand(user.Id, $"00000{i}"), CancellationToken.None);

        user.LockedUntil.Should().NotBeNull();
    }
}

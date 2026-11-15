using Application.Features.Users.Commands.Security.Confirm2FASetup;
using Application.Tests.Fakes;
using Domain.Users;
using Domain.Users.Errors;
using Domain.Users.ValueObjects;
using FluentAssertions;

namespace Application.Tests.Features.Users.Commands.Security.Confirm2FASetup;

/// <summary>
/// Use-case tests for Confirm2FASetupCommandHandler: the enabling of
/// 2FA must actually apply (the result of EnableTwoFactor is respected).
/// </summary>
public class Confirm2FASetupCommandHandlerTests
{
    private const string Secret = "SECRET32BASE";
    private const string ValidCode = "123456";

    private readonly FakeUserRepository _repo = new();
    private readonly FakeUnitOfWork _uow = new();
    private readonly FakeTotpService _totp = new();
    private readonly FakeAuditService _audit = new();
    private readonly Confirm2FASetupCommandHandler _handler;
    private readonly User _user;

    public Confirm2FASetupCommandHandlerTests()
    {
        _handler = new Confirm2FASetupCommandHandler(
            _repo, _uow, _totp, new TestHttpContextAccessor(), _audit);

        _user = User.Create(
            "identity-1",
            Email.Create("user@example.com").Value,
            Username.Create("username").Value,
            "John", "Doe", false, false).Value;
        _user.ConfirmEmail();
    }

    [Fact]
    public async Task Handle_ValidCode_ShouldEnableTwoFactor()
    {
        _totp.SetValid(Secret, ValidCode);
        await _repo.AddAsync(_user);

        var result = await _handler.Handle(new Confirm2FASetupCommand(_user.Id, ValidCode, Secret), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _user.IsTwoFactorEnabled.Should().BeTrue();
        _user.TwoFactorSecret.Should().Be(Secret);
    }

    [Fact]
    public async Task Handle_InvalidCode_ShouldFail_WithoutEnabling()
    {
        await _repo.AddAsync(_user);

        var result = await _handler.Handle(new Confirm2FASetupCommand(_user.Id, "000000", Secret), CancellationToken.None);

        result.Error.Should().Be(UserErrors.SecurityErrors.InvalidTotpCode);
        _user.IsTwoFactorEnabled.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_WhenAlreadyEnabled_ShouldFail_NotReportSuccess()
    {
        _user.EnableTwoFactor("OTHERSECRET");
        _totp.SetValid(Secret, ValidCode);
        await _repo.AddAsync(_user);

        var result = await _handler.Handle(new Confirm2FASetupCommand(_user.Id, ValidCode, Secret), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.SecurityErrors.TwoFactorAlreadyEnabled);
        // The original secret must not be overwritten by the failing attempt.
        _user.TwoFactorSecret.Should().Be("OTHERSECRET");
    }

    [Fact]
    public async Task Handle_DeactivatedUser_ShouldFail()
    {
        _user.Deactivate("policy");
        _totp.SetValid(Secret, ValidCode);
        await _repo.AddAsync(_user);

        var result = await _handler.Handle(new Confirm2FASetupCommand(_user.Id, ValidCode, Secret), CancellationToken.None);

        result.Error.Should().Be(UserErrors.Deactivated);
    }
}

using Application.Features.Users.Commands.Authentication.ResetPassword;
using Application.Tests.Fakes;
using Domain.Users;
using Domain.Users.Errors;
using Domain.Users.ValueObjects;
using FluentAssertions;

namespace Application.Tests.Features.Users.Commands.Authentication.ResetPassword;

/// <summary>
/// Use-case tests for ResetPasswordCommandHandler: token check, reuse
/// prevention, history update, and refresh-token revocation.
/// </summary>
public class ResetPasswordCommandHandlerTests
{
    private readonly FakeUserRepository _repo = new();
    private readonly FakeUnitOfWork _uow = new();
    private readonly FakeIdentityService _identity = new();
    private readonly FakeTokenService _tokens = new();
    private readonly FakePasswordHasher _hasher = new();
    private readonly FakeAuditService _audit = new();
    private readonly ResetPasswordCommandHandler _handler;
    private readonly User _user;

    public ResetPasswordCommandHandlerTests()
    {
        _handler = new ResetPasswordCommandHandler(
            _repo, _identity, _hasher, _tokens, _audit, _uow, new FakeTotpService(), new TestHttpContextAccessor());

        _user = User.Create(
            "identity-1",
            Email.Create("user@example.com").Value,
            Username.Create("username").Value,
            "John", "Doe", false, false).Value;
        _user.ConfirmEmail();
        _identity.Seed("identity-1", "user@example.com", "Password1!");
    }

    private ResetPasswordCommand Command(string @new = "NewPassword1!") => new(_user.Id, "reset-token", @new, TwoFactorCode: null);

    [Fact]
    public async Task Handle_ValidToken_ShouldUpdateHistory_AndRevokeRefreshToken()
    {
        await _repo.AddAsync(_user);

        var result = await _handler.Handle(Command(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _user.PasswordHashes.Should().Contain(_hasher.HashPassword("NewPassword1!"));
        _user.PasswordChangedAt.Should().NotBeNull();
        _tokens.RevokedUsers.Should().Contain("identity-1");
    }

    [Fact]
    public async Task Handle_ReusedPassword_ShouldFail()
    {
        var reusedHash = _hasher.HashPassword("NewPassword1!");
        _user.RecordPasswordChange(reusedHash, isPasswordReused: false);
        await _repo.AddAsync(_user);

        var result = await _handler.Handle(Command(), CancellationToken.None);

        result.Error.Should().Be(UserErrors.SecurityErrors.PasswordAlreadyUsed);
        _user.PasswordHashes.Count.Should().Be(1);
        _tokens.RevokedUsers.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_UnknownUser_ShouldFail()
    {
        var result = await _handler.Handle(Command(), CancellationToken.None);

        result.Error.Should().Be(UserErrors.NotFound);
    }
}

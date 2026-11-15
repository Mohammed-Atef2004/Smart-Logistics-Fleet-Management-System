using Application.Common.Settings;
using Application.Features.Users.Commands.Authentication.Login;
using Application.Features.Users.Services;
using Application.Tests.Fakes;
using Domain.Users;
using Domain.Users.Errors;
using Domain.Users.ValueObjects;
using FluentAssertions;
using Microsoft.Extensions.Options;

namespace Application.Tests.Features.Users.Commands.Authentication.Login;

/// <summary>
/// Use-case tests for LoginCommandHandler (without ASP.NET Identity):
/// enumeration, unconfirmed email, wrong password lockout, and the 2FA
/// branch that must NOT record a successful login.
/// </summary>
public class LoginCommandHandlerTests
{
    private readonly FakeUserRepository _repo;
    private readonly FakeUnitOfWork _uow;
    private readonly FakeIdentityService _identity;
    private readonly FakeTokenService _tokens;
    private readonly FakeAuditService _audit;
    private readonly FakeEmailService _email;
    private readonly TestHttpContextAccessor _http;
    private readonly LoginCommandHandler _handler;

    private const string Password = "Password1!";
    private const string UserEmail = "user@example.com";

    public LoginCommandHandlerTests()
    {
        _repo = new FakeUserRepository();
        _uow = new FakeUnitOfWork();
        _identity = new FakeIdentityService();
        _tokens = new FakeTokenService();
        _audit = new FakeAuditService();
        _email = new FakeEmailService();
        _http = new TestHttpContextAccessor();

        _handler = new LoginCommandHandler(
            _repo,
            _uow,
            _identity,
            _tokens,
            new FakeTotpService(),
            _email,
            Options.Create(new ApiSettings { BaseUrl = "https://localhost:7163" }),
            _http,
            _audit);
    }

    private User CreateConfirmedUser(bool twoFactor = false)
    {
        var user = User.Create(
            "identity-1",
            Email.Create(UserEmail).Value,
            Username.Create("username").Value,
            "John", "Doe",
            isEmailTaken: false,
            isUsernameTaken: false).Value;

        user.ConfirmEmail();
        _identity.Seed("identity-1", UserEmail, Password);

        if (twoFactor)
            user.EnableTwoFactor("SECRET32BASE");

        return user;
    }

    private static LoginCommand Command(string email = UserEmail, string password = Password)
        => new(email, password);

    [Fact]
    public async Task Handle_UnknownEmail_ShouldReturnInvalidCredentials_NotNotFound()
    {
        // Anti-enumeration: same error as a wrong password.
        var result = await _handler.Handle(Command(email: "ghost@example.com"), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.InvalidCredentials);
    }

    [Fact]
    public async Task Handle_WrongPassword_ShouldRecordFailedLogin()
    {
        var user = CreateConfirmedUser();
        await _repo.AddAsync(user);

        var result = await _handler.Handle(Command(password: "WrongPassword1!"), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.InvalidCredentials);
        user.FailedLoginAttempts.Should().Be(1);
    }

    [Fact]
    public async Task Handle_UnconfirmedEmail_ShouldSendReminder_AndFail()
    {
        var user = User.Create(
            "identity-1",
            Email.Create(UserEmail).Value,
            Username.Create("username").Value,
            "John", "Doe", false, false).Value;
        await _repo.AddAsync(user);
        _identity.Seed("identity-1", UserEmail, Password);

        var result = await _handler.Handle(Command(), CancellationToken.None);

        result.Error.Should().Be(UserErrors.EmailNotConfirmed);
        _email.SentCount.Should().Be(1);
    }

    [Fact]
    public async Task Handle_ValidCredentials_ShouldReturnTokens()
    {
        var user = CreateConfirmedUser();
        await _repo.AddAsync(user);

        var result = await _handler.Handle(Command(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.RequiresTwoFactor.Should().BeFalse();
        result.Value.Token.Should().NotBeNull();
        result.Value.RefreshToken.Should().NotBeNull();
        _tokens.AccessTokenCount.Should().Be(1);
        _tokens.RefreshTokenCount.Should().Be(1);
        _identity.UpdatedRefreshTokens.Should().ContainSingle(t => t.IdentityId == "identity-1");
        user.LastLoginAt.Should().NotBeNull();
        user.FailedLoginAttempts.Should().Be(0);
    }

    [Fact]
    public async Task Handle_TwoFactorUser_ShouldNotRecordLoginYet_AndNotIssueTokens()
    {
        var user = CreateConfirmedUser(twoFactor: true);
        await _repo.AddAsync(user);

        var result = await _handler.Handle(Command(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.RequiresTwoFactor.Should().BeTrue();
        result.Value.Token.Should().BeNull();
        result.Value.RefreshToken.Should().BeNull();

        // The login is not complete until the TOTP code is verified.
        user.LastLoginAt.Should().BeNull();
        user.DomainEvents.Should().NotContain(e =>
            e.GetType().Name == "UserLoggedInSuccessfullyDomainEvent");
        _tokens.AccessTokenCount.Should().Be(0);
    }

    [Fact]
    public async Task Handle_FiveWrongPasswords_ShouldLockAccount()
    {
        var user = CreateConfirmedUser();
        await _repo.AddAsync(user);

        for (var i = 0; i < 5; i++)
            await _handler.Handle(Command(password: $"Wrong{i}Pass1!"), CancellationToken.None);

        user.LockedUntil.Should().NotBeNull();

        // Even the correct password is rejected while locked.
        var result = await _handler.Handle(Command(), CancellationToken.None);
        result.IsFailure.Should().BeTrue();
    }
}

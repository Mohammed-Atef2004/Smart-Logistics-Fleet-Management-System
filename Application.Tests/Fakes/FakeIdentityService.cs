using Application.Common.Interfaces;
using Domain.SharedKernel;
using Domain.Users;

namespace Application.Tests.Fakes;

/// <summary>
/// In-memory IIdentityService. Stores plaintext passwords against the
/// "identity user" so CheckPasswordAsync behaves like the real one, and
/// records which identity users had their refresh token revoked.
/// </summary>
public sealed class FakeIdentityService : IIdentityService
{
    private readonly Dictionary<string, string> _passwordsByEmail = [];   // email -> password
    private readonly Dictionary<string, string> _emailsById = [];         // identityId -> email

    public List<string> RevokedRefreshTokens { get; } = [];
    public List<(string IdentityId, string RefreshToken)> UpdatedRefreshTokens { get; } = [];

    /// <summary>Seeds an identity user. Password hashing is out of scope here.</summary>
    public void Seed(string identityId, string email, string password)
    {
        _emailsById[identityId] = email;
        _passwordsByEmail[email] = password;
    }

    public Task<Result<string>> CreateUserAsync(string email, string userName, string password, CancellationToken ct = default)
    {
        var identityId = Guid.NewGuid().ToString();
        _emailsById[identityId] = email;
        _passwordsByEmail[email] = password;
        return Task.FromResult(Result<string>.Success(identityId));
    }

    public Task<Result> DeleteUserAsync(string identityId, CancellationToken ct = default) => Task.FromResult(Result.Success());

    public Task<Result> AssignRoleAsync(string identityId, UserRole role, CancellationToken ct = default) => Task.FromResult(Result.Success());
    public Task<Result> RemoveRoleAsync(string identityId, UserRole role, CancellationToken ct = default) => Task.FromResult(Result.Success());

    public Task<bool> CheckPasswordAsync(string email, string password, CancellationToken ct = default)
        => Task.FromResult(_passwordsByEmail.TryGetValue(email, out var stored) && stored == password);

    public Task<Result> ChangePasswordAsync(string identityId, string currentPassword, string newPassword, CancellationToken ct = default)
    {
        var email = _emailsById[identityId];
        if (_passwordsByEmail[email] != currentPassword)
            return Task.FromResult(Result.Failure(new Error("Identity.WrongPassword", "Current password is incorrect.")));
        _passwordsByEmail[email] = newPassword;
        return Task.FromResult(Result.Success());
    }

    public Task<Result> ResetPasswordAsync(string identityId, string token, string newPassword, CancellationToken ct = default)
    {
        var email = _emailsById[identityId];
        _passwordsByEmail[email] = newPassword;
        return Task.FromResult(Result.Success());
    }

    public Task<string> GeneratePasswordResetTokenAsync(string email, CancellationToken ct = default)
        => Task.FromResult("reset-token");

    public Task<bool> IsEmailConfirmedAsync(string identityId, CancellationToken ct = default) => Task.FromResult(true);
    public Task<string> GenerateEmailConfirmationTokenAsync(string identityId, CancellationToken ct = default) => Task.FromResult("confirm-token");
    public Task<Result> ConfirmEmailAsync(string identityId, string token, CancellationToken ct = default) => Task.FromResult(Result.Success());

    public Task<string> GenerateChangeEmailTokenAsync(string identityId, string newEmail, CancellationToken ct = default) => Task.FromResult("change-email-token");
    public Task<Result> ConfirmEmailChangeAsync(string identityId, string newEmail, string token, CancellationToken ct = default) => Task.FromResult(Result.Success());

    public Task UpdateRefreshTokenAsync(string identityId, string refreshToken, CancellationToken ct = default)
    {
        UpdatedRefreshTokens.Add((identityId, refreshToken));
        return Task.CompletedTask;
    }

    /// <summary>Allows tests to make CheckPasswordAsync fail (wrong password).</summary>
    public void SetPassword(string email, string password) => _passwordsByEmail[email] = password;
}

using Application.Common.Interfaces;
using Domain.SharedKernel;
using System.Security.Claims;
using System.Security.Principal;

namespace Application.Tests.Fakes;

public sealed class FakeTokenService : ITokenService
{
    public List<string> RevokedUsers { get; } = [];
    public int AccessTokenCount { get; private set; }
    public int RefreshTokenCount { get; private set; }

    public string GenerateAccessToken(TokenClaims claims)
    {
        AccessTokenCount++;
        return $"access-token-{AccessTokenCount}";
    }

    public string GenerateRefreshToken()
    {
        RefreshTokenCount++;
        return $"refresh-token-{RefreshTokenCount}";
    }

    public Task<TokenPair?> RotateRefreshTokenAsync(string refreshTokenValue, CancellationToken ct = default)
        => Task.FromResult<TokenPair?>(null);

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        => new ClaimsPrincipal(new ClaimsIdentity(Array.Empty<Claim>(), "Test"));

    public Task RevokeRefreshTokenAsync(string userId, CancellationToken ct = default)
    {
        RevokedUsers.Add(userId);
        return Task.CompletedTask;
    }
}

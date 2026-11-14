using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Application.Features.Users.Commands.Authentication.Logout;

public sealed class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result>
{
    private readonly ITokenService _tokenService;
    private readonly ITokenBlacklistService _blacklist;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LogoutCommandHandler(
        ITokenService tokenService,
        ITokenBlacklistService blacklist,
        IHttpContextAccessor httpContextAccessor)
    {
        _tokenService = tokenService;
        _blacklist = blacklist;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result> Handle(LogoutCommand request, CancellationToken ct)
    {
        var user = _httpContextAccessor.HttpContext?.User;

        // Get the IdentityId from the sub claim to remove the refresh token
        var identityId = user?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                         ?? user?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? user?.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

        if (string.IsNullOrEmpty(identityId))
            return Result.Failure(new Error("Auth.Error", "User Identity not found in Security Context."));

        // Get the JTI to add it to the blacklist
        var jti = user?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

        // Get the token expiry from the exp claim
        var expClaim = user?.FindFirst(JwtRegisteredClaimNames.Exp)?.Value;
        var tokenExpiry = expClaim is not null
            ? DateTimeOffset.FromUnixTimeSeconds(long.Parse(expClaim)).UtcDateTime
            : DateTime.UtcNow.AddMinutes(15); // fallback when the claim is missing

        // 1. Invalidate the access token immediately via the blacklist
        if (!string.IsNullOrEmpty(jti))
            _blacklist.Revoke(jti, tokenExpiry);

        // 2. Remove the refresh token from the database
        await _tokenService.RevokeRefreshTokenAsync(identityId, ct);

        return Result.Success();
    }
}
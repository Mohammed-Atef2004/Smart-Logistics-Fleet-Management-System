using Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Services;

public sealed class TokenBlacklistService : ITokenBlacklistService
{
    private readonly IMemoryCache _cache;
    private const string Prefix = "blacklist_jti_";

    public TokenBlacklistService(IMemoryCache cache)
    {
        _cache = cache;
    }

    // Keep the JTI in the cache until the token's natural expiry
    public void Revoke(string jti, DateTime tokenExpiry)
    {
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = tokenExpiry
        };
        _cache.Set(Prefix + jti, true, options);
    }

    public bool IsRevoked(string jti)
    {
        return _cache.TryGetValue(Prefix + jti, out _);
    }
}

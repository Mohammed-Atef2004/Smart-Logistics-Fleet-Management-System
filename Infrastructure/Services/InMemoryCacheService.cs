using Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Services
{
    public class InMemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;

        public InMemoryCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public T? Get<T>(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Cache key cannot be null or empty", nameof(key));

            _memoryCache.TryGetValue(key, out T value);
            return value;
        }

        public bool TryGetValue<T>(string key, out T value)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Cache key cannot be null or empty", nameof(key));

            return _memoryCache.TryGetValue(key, out value);
        }

        public void Set<T>(
            string key,
            T value,
            TimeSpan? absoluteExpiration = null,
            TimeSpan? slidingExpiration = null)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Cache key cannot be null or empty", nameof(key));

            var options = new MemoryCacheEntryOptions();

            if (absoluteExpiration.HasValue)
                options.AbsoluteExpirationRelativeToNow = absoluteExpiration;

            if (slidingExpiration.HasValue)
                options.SlidingExpiration = slidingExpiration;

            _memoryCache.Set(key, value, options);
        }

        public void Remove(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Cache key cannot be null or empty", nameof(key));

            _memoryCache.Remove(key);
        }
    }
}
using Gizmosbuy.BAL.Interfaces;
using Gizmosbuy.Core.Interfaces;
using Gizmosbuy.Core.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Gizmosbuy.BAL.Repository
{
    public class CacheService : ICacheService
    {
        private IMemoryCache _cache;
        public readonly IWebConfiguration _webConfiguration;

        public CacheService(IMemoryCache cache, IOptions<WebConfiguration> webConfiguration)
        {
            _cache = cache;
            _webConfiguration = webConfiguration.Value;
        }

        private MemoryCacheEntryOptions _memoryCacheEntryOptions = null;

        private MemoryCacheEntryOptions CreateDefaultOptions()
        {
            if (_memoryCacheEntryOptions == null)
            {
                int cacheTimeoutMinutes = _webConfiguration.MemoryCacheTimeoutMinutes > 0 ? _webConfiguration.MemoryCacheTimeoutMinutes : 60;
                int absoluteExpirationMinutes = cacheTimeoutMinutes;
                int slidingExpirationMinutes = (cacheTimeoutMinutes / 2); // Use half of absolute timeout for sliding expiration

                _memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(absoluteExpirationMinutes))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(slidingExpirationMinutes));
            }

            return _memoryCacheEntryOptions;
        }

        // Add specific cache entry
        public T GetOrSet<T>(string key, Func<ICacheEntry, T> createItem)
        {
            return _cache.GetOrCreate(key, entry =>
            {
                entry.SetOptions(CreateDefaultOptions());
                return createItem(entry);
            });
        }

        // Add specific cache entry with async
        public async Task<T> GetOrSetAsync<T>(string key, Func<ICacheEntry, Task<T>> createItem)
        {
            return await _cache.GetOrCreateAsync(key, entry =>
            {
                entry.SetOptions(CreateDefaultOptions());
                return createItem(entry);
            });
        }

        // Clear specific cache entry
        public void ClearCache(string key)
        {
            _cache.Remove(key);
        }

        // Clear all cache (requires custom logic)
        public void ClearAllCache(IEnumerable<string> keys)
        {
            foreach (var key in keys)
            {
                _cache.Remove(key);
            }
            _memoryCacheEntryOptions = null;
        }
        public void ClearAll()
        {
            // Dispose and recreate cache to clear everything
            _memoryCacheEntryOptions = null;
            _cache.Dispose();
            _cache = new MemoryCache(new MemoryCacheOptions());
        }


    }
}

using System.Text.Json;
using Gizmosbuy.BAL.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Gizmosbuy.BAL.Repository
{
    public class CacheService : ICacheService
    {
        private IMemoryCache _cache;

        public CacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        private MemoryCacheEntryOptions CreateDefaultOptions()
        {
            return new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(1))
                .SetSlidingExpiration(TimeSpan.FromSeconds(30));
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
        }
        public void ClearAll()
        {
            // Dispose and recreate cache to clear everything
            _cache.Dispose();
            _cache = new MemoryCache(new MemoryCacheOptions());
        }


    }
}

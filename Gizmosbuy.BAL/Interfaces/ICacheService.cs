using Microsoft.Extensions.Caching.Memory;

namespace Gizmosbuy.BAL.Interfaces
{
    public interface ICacheService
    {
        T GetOrSet<T>(string key, Func<ICacheEntry, T> createItem);
        Task<T> GetOrSetAsync<T>(string key, Func<ICacheEntry, Task<T>> createItem);
        void ClearCache(string key);
        void ClearAllCache(IEnumerable<string> keys);
        void ClearAll();
    }
}

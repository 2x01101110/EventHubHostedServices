using EventHubHostedServices.BuildingBlocks.Contracts;
using Microsoft.Extensions.Caching.Memory;

namespace EventHubHostedServices.BuildingBlocks.Infrastructure
{
    public class CachingService : ICachingService
    {
        private readonly IMemoryCache _cache;

        public CachingService(IMemoryCache cache)
        {
            this._cache = cache;
        }

        public T GetValue<T>(object key) => this._cache.Get<T>(key);

        public void RemoveValue(object key)
        {
            this._cache.Remove(key);
        }

        public void SetValue<T>(object key, T value)
        {
            this._cache.Set(key, value);
        }

        public bool TryGetValue<T>(object key, out T value)
        {
            value = this._cache.Get<T>(key);
            return value != null;
        }
    }
}

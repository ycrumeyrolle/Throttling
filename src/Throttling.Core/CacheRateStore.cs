using System.Threading.Tasks;
using Microsoft.Framework.Caching.Memory;

namespace Throttling
{
    public class CacheRateStore : IRateStore
    {
        private readonly IMemoryCache _cache;

        public CacheRateStore(IMemoryCache cache)
        {
            _cache = cache;
        }

        public Task<RemainingRate> GetRemainingRateAsync(string category, string endpoint, string key)
        {
            RemainingRate value;
            var computedKey = category + ":" + endpoint + ":" + key;

            value = _cache.Get<RemainingRate>(computedKey);

            return Task.FromResult(value);
        }

        public Task SetRemainingRateAsync(string category, string endpoint, string key, RemainingRate remaining)
        {
            _cache.Set(category + ":" + endpoint + ":" + key, remaining, new MemoryCacheEntryOptions { AbsoluteExpiration = remaining.Reset });
            return Constants.CompletedTask;
        }
    }
}
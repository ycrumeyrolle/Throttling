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

        public Task<RemainingRate> GetRemainingRateAsync(string category, string key)
        {
            RemainingRate value;
            var computedKey = category + ":" + key;

            value = _cache.Get<RemainingRate>(computedKey);

            return Task.FromResult(value);
        }

        public Task SetRemainingRateAsync(string category, string key, RemainingRate remaining)
        {
            _cache.Set(category + ":" + key, remaining, ctx =>
            {
                ctx.SetAbsoluteExpiration(remaining.Reset);
                return ctx.State;
            });
            return Constants.CompletedTask;
        }
    }
}
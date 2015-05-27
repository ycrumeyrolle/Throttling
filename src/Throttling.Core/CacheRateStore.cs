using System.Threading.Tasks;
using Microsoft.Framework.Caching.Memory;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public class CacheRateStore : IRateStore
    {
        private readonly IMemoryCache _cache;
        private readonly ISystemClock _clock;

        public CacheRateStore([NotNull] IMemoryCache cache, [NotNull] ISystemClock clock)
        {
            _cache = cache;
            _clock = clock;
        }

        public Task<RemainingRate> DecrementRemainingRateAsync([NotNull] string key, LimitRateRequirement requirement, long decrementValue)
        {
            RemainingRate rate = _cache.Get<RemainingRate>(key);
            if (rate == null)
            {
                rate = new RemainingRate
                {
                    Reset = _clock.UtcNow.Add(requirement.RenewalPeriod),
                    RemainingCalls = requirement.Calls
                };
            }
            else if (requirement.Sliding)
            {
                rate.Reset = _clock.UtcNow.Add(requirement.RenewalPeriod);
            }

            rate.RemainingCalls -= decrementValue;
            if (rate.RemainingCalls < 0)
            {
                rate.LimitReached = true;
                rate.RemainingCalls = 0;
            }

            _cache.Set(key, rate, new MemoryCacheEntryOptions { AbsoluteExpiration = rate.Reset });

            return Task.FromResult(rate);
        }
    }
}
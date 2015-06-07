using System;
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

        public Task<RemainingRate> DecrementRemainingRateAsync([NotNull] string key, [NotNull] ThrottleRequirement requirement, long decrementValue, bool reachLimitAtZero = false)
        {
            RemainingRate rate = _cache.Get<RemainingRate>(key);
            if (rate == null)
            {
                rate = new RemainingRate(reachLimitAtZero)
                {
                    Reset = _clock.UtcNow.Add(requirement.RenewalPeriod),
                    RemainingCalls = requirement.MaxValue
                };
            }
            else if (requirement.Sliding)
            {
                rate.Reset = _clock.UtcNow.Add(requirement.RenewalPeriod);
            }

            rate.RemainingCalls -= decrementValue;

            _cache.Set(key, rate, new MemoryCacheEntryOptions { AbsoluteExpiration = rate.Reset });

            return Task.FromResult(rate);
        }

        public Task<RemainingRate> GetRemainingRateAsync([NotNull] string key, [NotNull] ThrottleRequirement requirement)
        {
            return Task.FromResult(GetRemainingRate(key, requirement));
        }

        private RemainingRate GetRemainingRate(string key, ThrottleRequirement requirement)
        {
            RemainingRate rate = _cache.Get<RemainingRate>(key);
            if (rate == null)
            {
                rate = new RemainingRate(false)
                {
                    Reset = _clock.UtcNow.Add(requirement.RenewalPeriod),
                    RemainingCalls = requirement.MaxValue
                };
            }
            else if (requirement.Sliding)
            {
                rate.Reset = _clock.UtcNow.Add(requirement.RenewalPeriod);
            }

            return rate;
        }
    }
}
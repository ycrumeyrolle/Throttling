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
            RemainingRate rate;
            InMemoryRemainingRate inMemoryRate = _cache.Get<InMemoryRemainingRate>(key);
            if (inMemoryRate == null)
            {
                inMemoryRate = new InMemoryRemainingRate
                {
                    Reset = _clock.UtcNow.Add(requirement.RenewalPeriod),
                    RemainingCalls = requirement.MaxValue
                };
            }
            else if (requirement.Sliding)
            {
                inMemoryRate.Reset = _clock.UtcNow.Add(requirement.RenewalPeriod);
            }

            rate = new RemainingRate(reachLimitAtZero)
            {
                Reset = inMemoryRate.Reset,
                RemainingCalls = inMemoryRate.RemainingCalls - decrementValue
            };

            inMemoryRate.RemainingCalls = rate.RemainingCalls;

            _cache.Set(key, inMemoryRate, new MemoryCacheEntryOptions { AbsoluteExpiration = inMemoryRate.Reset });

            return Task.FromResult(rate);
        }

        public Task<RemainingRate> GetRemainingRateAsync([NotNull] string key, [NotNull] ThrottleRequirement requirement)
        {
            return Task.FromResult(GetRemainingRate(key, requirement));
        }

        private RemainingRate GetRemainingRate(string key, ThrottleRequirement requirement)
        {
            RemainingRate rate;
            InMemoryRemainingRate inMemoryRate = _cache.Get<InMemoryRemainingRate>(key);
            if (inMemoryRate == null)
            {
                inMemoryRate = new InMemoryRemainingRate
                {
                    Reset = _clock.UtcNow.Add(requirement.RenewalPeriod),
                    RemainingCalls = requirement.MaxValue
                };
            }
            else if (requirement.Sliding)
            {
                inMemoryRate.Reset = _clock.UtcNow.Add(requirement.RenewalPeriod);
            }

            rate = new RemainingRate(true)
            {
                Reset = inMemoryRate.Reset,
                RemainingCalls = inMemoryRate.RemainingCalls
            };

            return rate;
        }

        private class InMemoryRemainingRate
        {
            public long RemainingCalls { get; internal set; }
            public DateTimeOffset Reset { get; internal set; }
        }
    }
}
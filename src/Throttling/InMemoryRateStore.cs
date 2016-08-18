using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;

namespace Throttling
{
    public class MemoryThrottlingCounterStore : IThrottleCounterStore
    {
        private readonly IMemoryCache _cache;

        private readonly ISystemClock _clock;

        public MemoryThrottlingCounterStore(IMemoryCache cache, ISystemClock clock)
        {
            _cache = cache;
            _clock = clock;
        }

        public Task<ThrottleCounter> GetAsync(string key, ThrottleRequirement requirement)
        {
            ThrottleCounter counter;
            ThrottleCounterEntry item = _cache.Get<ThrottleCounterEntry>(key);
            if (item == null)
            {
                counter = new ThrottleCounter(_clock.UtcNow.Add(requirement.RenewalPeriod));
            }
            else
            {
                bool limitReached = false;
                if (item.Value >= requirement.MaxValue)
                {
                    limitReached = true;
                }

                counter = new ThrottleCounter(item.Reset, item.Value, limitReached);
            }

            return Task.FromResult(counter);
        }

        public Task<ThrottleCounter> IncrementAsync(string key, ThrottleRequirement requirement, long incrementValue = 1, bool reachLimitAtMax = false)
        {
            bool limitReached = false;
            ThrottleCounterEntry entry = _cache.Get<ThrottleCounterEntry>(key);
            if (entry == null)
            {
                entry = new ThrottleCounterEntry
                {
                    Reset = _clock.UtcNow.Add(requirement.RenewalPeriod),
                    Value = incrementValue
                };
            }
            else
            {
                var value = entry.Value + incrementValue;
                if (value > requirement.MaxValue || (reachLimitAtMax && value == requirement.MaxValue))
                {
                    limitReached = true;
                }

                entry.Value = value;

                if (requirement.Sliding)
                {
                    entry.Reset = _clock.UtcNow.Add(requirement.RenewalPeriod);
                }
            }

            _cache.Set(key, entry, entry.Reset);

            var counter = new ThrottleCounter(entry.Reset, entry.Value, limitReached);

            return Task.FromResult(counter);
        }

        private class ThrottleCounterEntry
        {
            public DateTimeOffset Reset { get; set; }

            public long Value { get; set; }
        }
    }
}
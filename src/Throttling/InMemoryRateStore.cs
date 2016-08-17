using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Internal;

namespace Throttling
{
    //public class MemoryRemainingRateStore : IRemainingRateStore
    //{
    //    private readonly IMemoryCache _cache;

    //    private readonly ISystemClock _clock;

    //    public Task<RemainingRate> GetAsync(string key, ThrottleRequirement requirement)
    //    {
    //        RemainingRate rate;
    //        RemainingRateItem item = _cache.Get<RemainingRateItem>(key);
    //        if (item == null)
    //        {
    //            item = new RemainingRateItem
    //            {
    //                Reset = _clock.UtcNow.Add(requirement.RenewalPeriod),
    //                RemainingCalls = requirement.MaxValue
    //            };
    //        }
    //        else if (requirement.Sliding)
    //        {
    //            inMemoryRate.Reset = _clock.UtcNow.Add(requirement.RenewalPeriod);
    //        }

    //        rate = new RemainingRate(true)
    //        {
    //            Reset = inMemoryRate.Reset,
    //            RemainingCalls = inMemoryRate.RemainingCalls
    //        };

    //        return rate;
    //    }

    //    public Task<RemainingRate> IncrementAsync(RemainingRateKey key, ThrottleRequirement requirement, long incrementValue = 1, bool reachLimitAtMax = false)
    //    {
         
    //    }

    //    private class RemainingRateItem
    //    {

    //    }
    //}

    public class InMemoryRateStore : IRateStore
    {
        private readonly IMemoryCache _cache;
        private readonly ISystemClock _clock;

        public InMemoryRateStore(IMemoryCache cache, ISystemClock clock)
        {
            if (cache == null)
            {
                throw new ArgumentNullException(nameof(cache));
            }

            if (clock == null)
            {
                throw new ArgumentNullException(nameof(clock));
            }

            _cache = cache;
            _clock = clock;
        }

        public Task<RemainingRate> DecrementRemainingRateAsync(string key, ThrottleRequirement requirement, long decrementValue, bool reachLimitAtZero = false)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }

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

        public Task<RemainingRate> GetRemainingRateAsync(string key, ThrottleRequirement requirement)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }

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

        public struct RemainingRateKey : IEquatable<RemainingRateKey>
        {
            public RemainingRateKey(Type requirementType)
            {
                RequirementType = requirementType;
            }

            public Type RequirementType { get; }

            public bool Equals(RemainingRateKey other)
            {
                return RequirementType.Equals(other.RequirementType);
            }

            public override bool Equals(object obj)
            {
                return Equals((RemainingRateKey)obj);
            }

            public override int GetHashCode()
            {
                var hashCodeCombiner = HashCodeCombiner.Start();

                hashCodeCombiner.Add(RequirementType);

                return hashCodeCombiner;
            }
        }
}
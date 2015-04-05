using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public abstract class RateLimitPolicy : IThrottlingPolicy
    {
        protected readonly TimeSpan _renewalPeriod;
        protected readonly long _calls;
        protected readonly bool _sliding;
        protected ThrottlingOptions _options;

        public string Category { get; set; }

        public IEnumerable<string> HttpMethods { get; set; }

        public RateLimitPolicy(long calls, TimeSpan renewalPeriod, bool sliding)
        {
            _calls = calls;
            _renewalPeriod = renewalPeriod;
            _sliding = sliding;
        }

        public virtual async Task<IEnumerable<ThrottlingResult>> EvaluateAsync(HttpContext context)
        {
            var result = new ThrottlingResult();
            var key = GetKey(context);

            if (key == null)
            {
                throw new InvalidOperationException("The current policy do not provide a key for the current context.");
            }

            var rate = await _options.RateStore.GetRemainingRateAsync(Category, key);
            if (rate == null)
            {
                rate = new RemainingRate
                {
                    Reset = _options.Clock.UtcNow.Add(_renewalPeriod),
                    Remaining = _calls
                };
            }

            var reset = _sliding ? _options.Clock.UtcNow.Add(_renewalPeriod) : rate.Reset;
            result.Reset = reset;
            AddRateLimitHeaders(rate, result.RateLimitHeaders);
            result.LimitReached = rate.Remaining <= 0;

            rate.Remaining = Math.Max(rate.Remaining - 1, 0);
            rate.Reset = reset;

            await _options.RateStore.SetRemainingRateAsync(Category, key, rate);

            return new[] { result };
        }

        public virtual void Configure(ThrottlingOptions options)
        {
            _options = options;
        }

        public abstract void AddRateLimitHeaders(RemainingRate rate, IDictionary<string, string> rateLimitHeaders);

        public abstract string GetKey([NotNull] HttpContext context);
    }
}
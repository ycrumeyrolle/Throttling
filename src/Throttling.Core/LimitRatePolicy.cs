using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public abstract class LimitRatePolicy : IThrottlingPolicy
    {
        protected readonly TimeSpan _window;
        protected readonly long _limit;
        protected readonly bool _sliding;
        protected readonly ThrottlingOptions _options;

        public abstract string Category { get; }

        public LimitRatePolicy(ThrottlingOptions options, long limit, TimeSpan window, bool sliding)
        {
            _limit = limit;
            _window = window;
            _sliding = sliding;
            _options = options;
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
                    Reset = _options.Clock.UtcNow.Add(_window),
                    Remaining = _limit
                };
            }

            result.Reset = rate.Reset;
            AddRateLimitHeaders(rate, result.RateLimitHeaders);
            result.LimitReached = rate.Remaining <= 0;

            rate.Remaining = rate.Remaining - 1;

            await _options.RateStore.SetRemainingRateAsync(Category, key, rate);

            return new[] { result };
        }

        public abstract void AddRateLimitHeaders(RemainingRate rate, IDictionary<string, string> rateLimitHeaders);

        public abstract string GetKey([NotNull] HttpContext context);
    }
}
using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public abstract class RateLimitHandler<TRequirement> : ThrottlingHandler<TRequirement> where TRequirement : LimitRateRequirement
    {
        private readonly IRateStore _store;

        public RateLimitHandler([NotNull] IRateStore store)
        {
            _store = store;
        }

        public override async Task HandleAsync([NotNull] ThrottlingContext context, [NotNull] TRequirement requirement)
        {
            var key = GetKey(context.HttpContext, requirement);
            if (key == null)
            {
                throw new InvalidOperationException("The current throttling requirement do not provide a key for the current context.");
            }

            key = typeof(TRequirement) + key;
            var rate = await _store.DecrementRemainingRateAsync(key, requirement, 1);
            if (rate.LimitReached)
            {
                context.TooManyRequest(requirement, rate.Reset);
            }

            AddRateLimitHeaders(rate, context, requirement);
        }

        public abstract void AddRateLimitHeaders([NotNull] RemainingRate rate, [NotNull] ThrottlingContext context, [NotNull] TRequirement requirement);

        public abstract string GetKey([NotNull] HttpContext httpContext, [NotNull] TRequirement requirement);
    }
}
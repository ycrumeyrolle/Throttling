using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public abstract class InboundHandler<TRequirement> : ThrottlingHandler<TRequirement> where TRequirement : ThrottlingRequirement
    {
        private readonly IRateStore _store;

        public InboundHandler([NotNull] IRateStore store)
        {
            _store = store;
        }

        public override async Task HandleAsync([NotNull] ThrottlingContext throttlingContext, [NotNull] TRequirement requirement)
        {
            var key = GetKey(throttlingContext.HttpContext, requirement);
            if (key == null)
            {
                return;
            }

            key = typeof(TRequirement) + key;
            var rate = await _store.DecrementRemainingRateAsync(key, requirement, 1);
            if (rate.LimitReached)
            {
                throttlingContext.TooManyRequest(requirement, rate.Reset);
            }
            else
            {
                throttlingContext.Succeed(requirement);
            }

            AddRateLimitHeaders(rate, throttlingContext, requirement);
        }

        public abstract void AddRateLimitHeaders([NotNull] RemainingRate rate, [NotNull] ThrottlingContext throttlingContext, [NotNull] TRequirement requirement);

        public abstract string GetKey([NotNull] HttpContext httpContext, [NotNull] TRequirement requirement);
    }
}
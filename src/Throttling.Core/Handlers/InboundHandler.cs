using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public abstract class InboundHandler<TRequirement> : ThrottleHandler<TRequirement> where TRequirement : ThrottleRequirement
    {
        private readonly IRateStore _store;

        public InboundHandler([NotNull] IRateStore store)
        {
            _store = store;
        }

        public override async Task HandleAsync([NotNull] ThrottleContext throttleContext, [NotNull] TRequirement requirement)
        {
            var key = GetKey(throttleContext.HttpContext, requirement);
            if (key == null)
            {
                return;
            }

            key = typeof(TRequirement) + key;
            var rate = await _store.DecrementRemainingRateAsync(key, requirement, 1);
            if (rate.LimitReached)
            {
                throttleContext.TooManyRequest(requirement, rate.Reset);
            }
            else
            {
                throttleContext.Succeed(requirement);
            }

            AddRateLimitHeaders(rate, throttleContext, requirement);
        }

        public abstract void AddRateLimitHeaders([NotNull] RemainingRate rate, [NotNull] ThrottleContext throttleContext, [NotNull] TRequirement requirement);

        public abstract string GetKey([NotNull] HttpContext httpContext, [NotNull] TRequirement requirement);
    }
}
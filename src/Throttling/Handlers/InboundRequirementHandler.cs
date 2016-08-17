using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Throttling
{
    public abstract class InboundRequirementHandler<TRequirement> : RequirementHandler<TRequirement> where TRequirement : ThrottleRequirement
    {
        private readonly IRateStore _store;

        public InboundRequirementHandler(IRateStore store)
        {
            _store = store;
        }

        public override async Task HandleRequirementAsync(ThrottleContext throttleContext, TRequirement requirement)
        {
            if (throttleContext == null)
            {
                throw new ArgumentNullException(nameof(throttleContext));
            }

            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }


            var key = GetKey(throttleContext.HttpContext, requirement);
            if (key == null)
            {
                throttleContext.Skipped(requirement);
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

        public abstract void AddRateLimitHeaders(RemainingRate rate, ThrottleContext throttleContext, TRequirement requirement);

        public abstract string GetKey(HttpContext httpContext, TRequirement requirement);
    }
}
using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;

namespace Throttling
{
    public abstract class OutboundRequirementHandler<TRequirement> : RequirementHandler<TRequirement> where TRequirement : ThrottleRequirement
    {
        private readonly IRateStore _store;

        public OutboundRequirementHandler( IRateStore store)
        {
            if (store == null)
            {
                throw new ArgumentNullException(nameof(store));
            }

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
            var rate = await _store.GetRemainingRateAsync(key, requirement);
            if (rate.LimitReached)
            {
                throttleContext.TooManyRequest(requirement, rate.Reset);
            }
            else
            {
                throttleContext.Succeed(requirement);
            }

            throttleContext.HttpContext.Response.TrackContentLength(throttleContext);
        }

        public override async Task PostHandleRequirementAsync(ThrottleContext throttleContext, TRequirement requirement)
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
                return;
            }
            
            key = typeof(TRequirement) + key;
            var decrementValue = GetDecrementValue(throttleContext, requirement);
            await _store.DecrementRemainingRateAsync(key, requirement, decrementValue, reachLimitAtZero: true);
        }

        public abstract string GetKey(HttpContext httpContext, TRequirement requirement);

        public abstract long GetDecrementValue(ThrottleContext throttleContext, TRequirement requirement);
    }
}
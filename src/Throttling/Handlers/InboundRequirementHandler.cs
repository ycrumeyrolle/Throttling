using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Throttling
{
    public abstract class InboundRequirementHandler<TRequirement> : RequirementHandler<TRequirement> where TRequirement : ThrottleRequirement
    {
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
            var counter = await throttleContext.Store.IncrementAsync(key, requirement, 1);
            if (counter.LimitReached)
            {
                throttleContext.TooManyRequest(requirement, counter.Reset);
            }
            else
            {
                throttleContext.Succeed(requirement);
            }

            AddRateLimitHeaders(counter, throttleContext, requirement);
        }

        public abstract void AddRateLimitHeaders(ThrottleCounter counter, ThrottleContext throttleContext, TRequirement requirement);

        public abstract string GetKey(HttpContext httpContext, TRequirement requirement);
    }
}
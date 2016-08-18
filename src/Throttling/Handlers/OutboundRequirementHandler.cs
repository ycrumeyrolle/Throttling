using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Throttling
{
    public abstract class OutboundRequirementHandler<TRequirement> : RequirementHandler<TRequirement> where TRequirement : ThrottleRequirement
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
            var counter = await throttleContext.Store.GetAsync(key, requirement);
            if (counter.LimitReached)
            {
                throttleContext.TooManyRequest(requirement, counter.Reset);
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
            var incrementValue = GetIncrementValue(throttleContext, requirement);
            await throttleContext.Store.IncrementAsync(key, requirement, incrementValue, reachLimitAtMax: true);
        }

        public abstract string GetKey(HttpContext httpContext, TRequirement requirement);

        public abstract long GetIncrementValue(ThrottleContext throttleContext, TRequirement requirement);
    }
}
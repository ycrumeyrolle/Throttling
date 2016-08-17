using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Internal;

namespace Throttling
{
    public class IPSessionRateLimitHandler : InboundRequirementHandler<IPSessionRateLimitRequirement>
    {
        public IPSessionRateLimitHandler(IRateStore store)
            : base(store)
        {
        }

        public override void AddRateLimitHeaders(RemainingRate rate, ThrottleContext throttleContext, IPSessionRateLimitRequirement requirement)
        {
            if (rate == null)
            {
                throw new ArgumentNullException(nameof(rate));
            }

            if (throttleContext == null)
            {
                throw new ArgumentNullException(nameof(throttleContext));
            }

            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }

            throttleContext.ResponseHeaders["X-RateLimit-IPLimit"] = requirement.MaxValue.ToString();
            throttleContext.ResponseHeaders["X-RateLimit-IPRemaining"] = rate.RemainingCalls.ToString();
        }

        public override string GetKey(HttpContext httpContext, IPSessionRateLimitRequirement requirement)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }

            var sessionId = httpContext.Request.Headers["throttling_sessionId"];
            if (sessionId.Count == 0)
            {
                return null;
            }

            return sessionId;
        }

        public override Task PostHandleRequirementAsync(ThrottleContext throttleContext, IPSessionRateLimitRequirement requirement)
        {
            if (throttleContext == null)
            {
                throw new ArgumentNullException(nameof(throttleContext));
            }

            if (requirement == null)
            {
                throw new ArgumentNullException(nameof(requirement));
            }

            return base.PostHandleRequirementAsync(throttleContext, requirement);
        }
    }
}
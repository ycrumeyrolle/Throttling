using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Throttling
{
    public class IPSessionRateLimitHandler : InboundRequirementHandler<IPSessionRateLimitRequirement>
    {
        public override void AddRateLimitHeaders(ThrottleCounter counter, ThrottleContext throttleContext, IPSessionRateLimitRequirement requirement)
        {
            if (counter == null)
            {
                throw new ArgumentNullException(nameof(counter));
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
            throttleContext.ResponseHeaders["X-RateLimit-IPRemaining"] = counter.Remaining(requirement).ToString();
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
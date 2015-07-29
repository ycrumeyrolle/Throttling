using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public class IPRateLimitHandler : InboundRequirementHandler<IPRateLimitRequirement>
    {
        public IPRateLimitHandler(IRateStore store)
            : base(store)
        {
        }

        public override void AddRateLimitHeaders([NotNull] RemainingRate rate, [NotNull] ThrottleContext throttleContext, [NotNull] IPRateLimitRequirement requirement)
        {
            throttleContext.ResponseHeaders.Set("X-RateLimit-IPLimit", requirement.MaxValue.ToString());
            throttleContext.ResponseHeaders.Set("X-RateLimit-IPRemaining", rate.RemainingCalls.ToString());
        }

        public override string GetKey([NotNull] HttpContext httpContext, [NotNull] IPRateLimitRequirement requirement)
        {
            return requirement.GetKey(httpContext);
        }
    }
}
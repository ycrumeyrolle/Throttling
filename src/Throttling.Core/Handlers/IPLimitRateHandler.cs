using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public class IPRateLimitHandler : InboundHandler<IPRateLimitRequirement>
    {
        public IPRateLimitHandler(IRateStore store)
            : base(store)
        {
        }

        public override void AddRateLimitHeaders([NotNull] RemainingRate rate, [NotNull] ThrottlingContext throttlingContext, [NotNull] IPRateLimitRequirement requirement)
        {
            throttlingContext.Headers.Set("X-RateLimit-IPLimit", requirement.MaxValue.ToString());
            throttlingContext.Headers.Set("X-RateLimit-IPRemaining", rate.RemainingCalls.ToString());
        }

        public override string GetKey([NotNull] HttpContext httpContext, [NotNull] IPRateLimitRequirement requirement)
        {
            return requirement.GetKey(httpContext);
        }
    }
}
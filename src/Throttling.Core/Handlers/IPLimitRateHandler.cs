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

        public override void AddRateLimitHeaders([NotNull] RemainingRate rate, [NotNull] ThrottlingContext context, [NotNull] IPRateLimitRequirement requirement)
        {
            context.Headers.Set("X-RateLimit-IPLimit", requirement.MaxValue.ToString());
            context.Headers.Set("X-RateLimit-IPRemaining", rate.RemainingCalls.ToString());
        }

        public override string GetKey([NotNull] HttpContext httpContext, [NotNull] IPRateLimitRequirement requirement)
        {
            return requirement.GetKey(httpContext);
        }
    }
}
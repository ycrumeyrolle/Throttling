using System.Globalization;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public class AuthenticatedUserRateLimitHandler : InboundHandler<AuthenticatedUserRateLimitRequirement>
    {
        public AuthenticatedUserRateLimitHandler([NotNull] IRateStore store)
            : base(store)
        {
        }

        public override void AddRateLimitHeaders([NotNull] RemainingRate rate, [NotNull] ThrottlingContext context, [NotNull] AuthenticatedUserRateLimitRequirement requirement)
        {
            context.Headers.Set("X-RateLimit-UserLimit", requirement.MaxValue.ToString(CultureInfo.InvariantCulture));
            context.Headers.Set("X-RateLimit-UserRemaining", rate.RemainingCalls.ToString(CultureInfo.InvariantCulture));
            context.Headers.Set("X-RateLimit-UserReset", rate.Reset.ToEpoch().ToString(CultureInfo.InvariantCulture));
        }

        public override string GetKey([NotNull] HttpContext httpContext, [NotNull] AuthenticatedUserRateLimitRequirement requirement)
        {
            return requirement.GetKey(httpContext);
        }
    }
}
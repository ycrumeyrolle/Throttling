using System.Globalization;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public class AuthenticatedUserRateLimitHandler : InboundRequirementHandler<AuthenticatedUserRateLimitRequirement>
    {
        public AuthenticatedUserRateLimitHandler([NotNull] IRateStore store)
            : base(store)
        {
        }

        public override void AddRateLimitHeaders([NotNull] RemainingRate rate, [NotNull] ThrottleContext throttleContext, [NotNull] AuthenticatedUserRateLimitRequirement requirement)
        {
            throttleContext.ResponseHeaders.Set("X-RateLimit-UserLimit", requirement.MaxValue.ToString(CultureInfo.InvariantCulture));
            throttleContext.ResponseHeaders.Set("X-RateLimit-UserRemaining", rate.RemainingCalls.ToString(CultureInfo.InvariantCulture));
            throttleContext.ResponseHeaders.Set("X-RateLimit-UserReset", rate.Reset.ToEpoch().ToString(CultureInfo.InvariantCulture));
        }

        public override string GetKey([NotNull] HttpContext httpContext, [NotNull] AuthenticatedUserRateLimitRequirement requirement)
        {
            return requirement.GetKey(httpContext);
        }
    }
}
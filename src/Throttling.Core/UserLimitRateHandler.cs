using System.Globalization;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public class UserLimitRateHandler : RateLimitHandler<UserLimitRateRequirement>
    {
        private readonly IPLimitRateHandler _unauthenticatedHandler;
        public UserLimitRateHandler([NotNull] IRateStore store)
            : base(store)
        {
            _unauthenticatedHandler = new IPLimitRateHandler(store);
        }

        public override void AddRateLimitHeaders([NotNull] RemainingRate rate, [NotNull] ThrottlingContext context, [NotNull] UserLimitRateRequirement requirement)
        {
            context.Headers.Set("X-RateLimit-UserLimit", requirement.Calls.ToString(CultureInfo.InvariantCulture));
            context.Headers.Set("X-RateLimit-UserRemaining", rate.RemainingCalls.ToString(CultureInfo.InvariantCulture));
            context.Headers.Set("X-RateLimit-UserReset", rate.Reset.ToEpoch().ToString(CultureInfo.InvariantCulture));
        }

        public override string GetKey([NotNull] HttpContext httpContext, [NotNull] UserLimitRateRequirement requirement)
        {
            if (httpContext.User.Identity.IsAuthenticated)
            {
                return httpContext.User.Identity.Name;
            }

            return _unauthenticatedHandler?.GetKey(httpContext, requirement.UnauthenticatedRequirement);
        }
    }
}
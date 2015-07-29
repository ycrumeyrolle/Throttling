using System.Globalization;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public class ApiKeyRateLimitHandler : InboundRequirementHandler<ApiKeyRateLimitRequirement>
    {
        public ApiKeyRateLimitHandler(IRateStore store)
            : base(store)
        {
        }

        public override void AddRateLimitHeaders([NotNull] RemainingRate rate, [NotNull] ThrottleContext throttleContext, [NotNull] ApiKeyRateLimitRequirement requirement)
        {
            throttleContext.ResponseHeaders.Set("X-RateLimit-ClientLimit", requirement.MaxValue.ToString(CultureInfo.InvariantCulture));
            throttleContext.ResponseHeaders.Set("X-RateLimit-ClientRemaining", rate.RemainingCalls.ToString(CultureInfo.InvariantCulture));
        }

        public override string GetKey([NotNull] HttpContext httpContext, [NotNull] ApiKeyRateLimitRequirement requirement)
        {
            return requirement.GetApiKey(httpContext);
        }
    }
}
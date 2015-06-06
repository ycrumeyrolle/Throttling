using System.Globalization;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public abstract class ApiKeyRateLimitHandler : InboundHandler<ApiKeyRateLimitRequirement>
    {
        protected ApiKeyRateLimitHandler(IRateStore store)
            : base(store)
        {
        }

        public override void AddRateLimitHeaders([NotNull] RemainingRate rate, [NotNull] ThrottlingContext context, [NotNull] ApiKeyRateLimitRequirement requirement)
        {
            context.Headers.Set("X-RateLimit-ClientLimit", requirement.MaxValue.ToString(CultureInfo.InvariantCulture));
            context.Headers.Set("X-RateLimit-ClientRemaining", rate.RemainingCalls.ToString(CultureInfo.InvariantCulture));
        }

        public override string GetKey([NotNull] HttpContext httpContext, [NotNull] ApiKeyRateLimitRequirement requirement)
        {
            return requirement.GetApiKey(httpContext);
        }
    }
}
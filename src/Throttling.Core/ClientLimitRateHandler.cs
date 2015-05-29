using System.Globalization;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public abstract class ClientLimitRateHandler<TRequirement> : RateLimitHandler<TRequirement> where TRequirement : LimitRateRequirement
    {
        private readonly IApiKeyProvider _apiKeyProvider;

        protected ClientLimitRateHandler(IRateStore store, IApiKeyProvider apiKeyProvider)
            : base(store)
        {
            _apiKeyProvider = apiKeyProvider;
        }

        public override void AddRateLimitHeaders([NotNull] RemainingRate rate, [NotNull] ThrottlingContext context, [NotNull] TRequirement requirement)
        {
            context.Headers.Set("X-RateLimit-ClientLimit", requirement.Calls.ToString(CultureInfo.InvariantCulture));
            context.Headers.Set("X-RateLimit-ClientRemaining", rate.RemainingCalls.ToString(CultureInfo.InvariantCulture));
        }

        public override string GetKey([NotNull] HttpContext httpContext, [NotNull] TRequirement requirement)
        {
            return _apiKeyProvider.GetApiKey(httpContext);
        }
    }
}
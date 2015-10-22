using System;

namespace Throttling
{
    public sealed class RouteApiKeyRateLimitRequirement : ApiKeyRateLimitRequirement
    {
        public RouteApiKeyRateLimitRequirement(long calls, TimeSpan renewalPeriod, bool sliding, string routeTemplate, string apiKeyName)
            : base (calls, renewalPeriod, sliding, new RouteApiKeyProvider(routeTemplate, apiKeyName))
        {
        }
    }
}
using System;

namespace Throttling
{
    public sealed class RouteApiKeyBandwidthRequirement : ApiKeyBandwidthRequirement
    {
        public RouteApiKeyBandwidthRequirement(long bandwidth, TimeSpan renewalPeriod, bool sliding, string routeTemplate, string apiKeyName)
            : base (bandwidth, renewalPeriod, sliding, new RouteApiKeyProvider(routeTemplate, apiKeyName))
        {
        }
    }
}
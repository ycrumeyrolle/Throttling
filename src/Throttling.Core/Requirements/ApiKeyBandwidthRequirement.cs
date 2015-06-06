using System;

namespace Throttling
{
    public abstract class ApiKeyBandwidthRequirement : ApiKeyRequirement
    {
        protected ApiKeyBandwidthRequirement(long maxValue, TimeSpan renewalPeriod, bool sliding, IApiKeyProvider apiKeyProvider)
            : base(maxValue, renewalPeriod, sliding, apiKeyProvider)
        {
        }
    }
}
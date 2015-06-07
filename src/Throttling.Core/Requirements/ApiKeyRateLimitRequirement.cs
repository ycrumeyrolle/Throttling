using System;

namespace Throttling
{
    public abstract class ApiKeyRateLimitRequirement : ApiKeyRequirement
    {
        protected ApiKeyRateLimitRequirement(long maxValue, TimeSpan renewalPeriod, bool sliding, IApiKeyProvider apiKeyProvider)
            : base(maxValue, renewalPeriod, sliding, apiKeyProvider)
        {
        }
    }
}
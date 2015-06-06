using System;

namespace Throttling
{
    public sealed class HeaderApiKeyRateLimitRequirement : ApiKeyRateLimitRequirement
    {
        public HeaderApiKeyRateLimitRequirement(long calls, TimeSpan renewalPeriod, bool sliding, string parameter)
            : base (calls, renewalPeriod, sliding, new HeaderApiKeyProvider(parameter))
        {
        }
    }
}
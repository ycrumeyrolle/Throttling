using System;

namespace Throttling
{
    public sealed class FormApiKeyRateLimitRequirement : ApiKeyRateLimitRequirement
    {
        public FormApiKeyRateLimitRequirement(long calls, TimeSpan renewalPeriod, bool sliding, string parameter)
            : base (calls, renewalPeriod, sliding, new FormApiKeyProvider(parameter))
        {
        }
    }
}
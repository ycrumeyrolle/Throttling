using System;

namespace Throttling
{
    public sealed class QueryStringApiKeyRateLimitRequirement : ApiKeyRequirement
    {
        public QueryStringApiKeyRateLimitRequirement(long calls, TimeSpan renewalPeriod, bool sliding, string parameter)
            : base(calls, renewalPeriod, sliding, new QueryStringApiKeyProvider(parameter))
        {
        }
    }
}
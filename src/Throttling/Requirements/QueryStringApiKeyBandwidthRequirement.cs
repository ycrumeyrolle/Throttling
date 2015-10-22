using System;

namespace Throttling
{
    public sealed class QueryStringApiKeyBandwidthRequirement : ApiKeyBandwidthRequirement
    {
        public QueryStringApiKeyBandwidthRequirement(long bandwidth, TimeSpan renewalPeriod, bool sliding, string parameter)
            : base (bandwidth, renewalPeriod, sliding, new QueryStringApiKeyProvider(parameter))
        {
        }
    }
}
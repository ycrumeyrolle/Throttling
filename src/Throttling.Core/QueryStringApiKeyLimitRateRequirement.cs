using System;

namespace Throttling
{
    public sealed class QueryStringApiKeyLimitRateRequirement : LimitRateRequirement
    {
        public QueryStringApiKeyLimitRateRequirement(long calls, TimeSpan renewalPeriod, bool sliding)
            : base (calls, renewalPeriod, sliding)
        {
        }
    }
}
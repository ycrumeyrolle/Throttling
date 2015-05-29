using System;

namespace Throttling
{
    public sealed class RouteApiKeyLimitRateRequirement : LimitRateRequirement
    {
        public RouteApiKeyLimitRateRequirement(long calls, TimeSpan renewalPeriod, bool sliding): base (calls, renewalPeriod, sliding)
        {
        }
    }
}
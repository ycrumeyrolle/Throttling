using System;

namespace Throttling
{
    public sealed class IPLimitRateRequirement : LimitRateRequirement
    {
        public IPLimitRateRequirement(long calls, TimeSpan renewalPeriod, bool sliding)
            : base (calls, renewalPeriod, sliding)
        {
        }
    }
}
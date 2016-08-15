using System;

namespace Throttling
{
    public class IPSessionRateLimitRequirement : ThrottleRequirement
    {
        public IPSessionRateLimitRequirement(long maxValue, TimeSpan renewalPeriod, bool sliding)
            : base (maxValue, renewalPeriod, sliding)
        {
        }
    }
}
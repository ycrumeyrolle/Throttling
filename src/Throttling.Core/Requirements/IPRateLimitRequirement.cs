using System;

namespace Throttling
{
    public class IPRateLimitRequirement : IPRequirement
    {
        public IPRateLimitRequirement(long calls, TimeSpan renewalPeriod, bool sliding)
            : base(calls, renewalPeriod, sliding)
        {
        }
    }
}
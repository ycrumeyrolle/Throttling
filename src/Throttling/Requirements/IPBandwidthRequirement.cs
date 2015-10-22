using System;

namespace Throttling
{
    public class IPBandwidthRequirement : IPRequirement
    {
        public IPBandwidthRequirement(long bandwidth, TimeSpan renewalPeriod, bool sliding)
            : base(bandwidth, renewalPeriod, sliding)
        {
        }
    }
}
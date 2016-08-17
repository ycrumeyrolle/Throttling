using System.Collections.Generic;
using Throttling.IPRanges;

namespace Throttling
{
    public class IPExclusion : IThrottleExclusion
    {
        public IPExclusion(params string[] ranges)
        {
            Whitelist = new IPWhitelist(ranges);
        }

        public IPWhitelist Whitelist { get; }
    }
}
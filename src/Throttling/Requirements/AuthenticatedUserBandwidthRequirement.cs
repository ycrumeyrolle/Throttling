using System;

namespace Throttling
{
    public sealed class AuthenticatedUserBandwidthRequirement : AuthenticatedUserRequirement
    {
        public AuthenticatedUserBandwidthRequirement(long bandwidth, TimeSpan renewalPeriod, bool sliding)
            : base(bandwidth, renewalPeriod, sliding)
        {
        }
    }
}
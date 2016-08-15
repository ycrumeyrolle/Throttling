using System;

namespace Throttling
{
    public sealed class AuthenticatedUserRateLimitRequirement : AuthenticatedUserRequirement
    {
        public AuthenticatedUserRateLimitRequirement(long calls, TimeSpan renewalPeriod, bool sliding)
            : base(calls, renewalPeriod, sliding)
        {
        }
    }
}
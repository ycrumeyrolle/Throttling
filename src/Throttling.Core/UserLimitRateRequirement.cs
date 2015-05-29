using System;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public sealed class UserLimitRateRequirement : LimitRateRequirement
    {
        public UserLimitRateRequirement(long calls, TimeSpan renewalPeriod, bool sliding, [NotNull] IPLimitRateRequirement unauthenticatedUserRequirement)
            : base(calls, renewalPeriod, sliding)
        {
            UnauthenticatedRequirement = unauthenticatedUserRequirement;
        }

        public IPLimitRateRequirement UnauthenticatedRequirement { get; }
    }
}
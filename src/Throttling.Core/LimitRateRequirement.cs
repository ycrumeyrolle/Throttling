using System;

namespace Throttling
{
    public abstract class LimitRateRequirement : IThrottlingRequirement
    {
        protected LimitRateRequirement(long calls, TimeSpan renewalPeriod, bool sliding)
        {
            Calls = calls;
            RenewalPeriod = renewalPeriod;
            Sliding = sliding;
        }

        public long Calls
        {
            get;
        }

        public TimeSpan RenewalPeriod
        {
            get;
        }

        public bool Sliding
        {
            get;
        }
    }
}
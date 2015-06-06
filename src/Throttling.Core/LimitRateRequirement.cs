using System;

namespace Throttling
{
    public abstract class ThrottlingRequirement : IThrottlingRequirement
    {
        protected ThrottlingRequirement(long maxValue, TimeSpan renewalPeriod, bool sliding)
        {
            MaxValue = maxValue;
            RenewalPeriod = renewalPeriod;
            Sliding = sliding;
        }

        public long MaxValue
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
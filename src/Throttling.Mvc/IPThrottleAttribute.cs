using System;
using Microsoft.AspNet.Mvc.ApplicationModels;

namespace Throttling.Mvc
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class IPThrottleAttribute : RateLimitAttribute
    {
        public IPThrottleAttribute(long calls, long renewalPeriod)
            : base(calls, renewalPeriod)
        {
        }

        protected override void ApplyCore(ActionModel model, ThrottlePolicyBuilder builder)
        {
            builder.LimitIPRate(Calls, TimeSpan.FromSeconds(RenewalPeriod), Sliding);
        }
    }
}
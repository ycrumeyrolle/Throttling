using System;
using Microsoft.AspNet.Mvc.ApplicationModels;

namespace Throttling.Mvc
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class AuthenticatedUserThrottleAttribute : RateLimitAttribute
    {
        public AuthenticatedUserThrottleAttribute(long calls, long renewalPeriod)
            : base(calls, renewalPeriod)
        {
        }

        protected override void ApplyCore(ActionModel model, ThrottlePolicyBuilder builder)
        {
            builder.LimitAuthenticatedUserRate(Calls, TimeSpan.FromSeconds(RenewalPeriod), Sliding);
        }
    }
}
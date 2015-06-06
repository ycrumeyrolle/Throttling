using System;
using Microsoft.AspNet.Mvc.ApplicationModels;

namespace Throttling.Mvc
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class AuthenticatedUserThrottlingAttribute : RateLimitAttribute
    {
        public AuthenticatedUserThrottlingAttribute(long calls, long renewalPeriod)
            : base(calls, renewalPeriod)
        {
        }

        protected override void ApplyCore(ActionModel model, ThrottlingPolicyBuilder builder)
        {
            builder.LimitAuthenticatedUserRate(Calls, TimeSpan.FromSeconds(RenewalPeriod), Sliding);
        }
    }
}
using System;
using Microsoft.AspNet.Mvc.ApplicationModels;

namespace Throttling.Mvc
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class AuthenticatedUserThrottlingAttribute : RateLimitAttribute
    {
        public AuthenticatedUserThrottlingAttribute(long authenticatedCalls, long authenticatedRenewalPeriod)
            : base(authenticatedCalls, authenticatedRenewalPeriod)
        {
        }

        protected override void ApplyCore(ActionModel model, ThrottlingPolicyBuilder builder)
        {
            builder.LimitAuthenticatedUserRate(Calls, TimeSpan.FromSeconds(RenewalPeriod), Sliding);
        }
    }
}
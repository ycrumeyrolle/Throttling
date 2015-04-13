using System;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ApplicationModels;

namespace Throttling.Mvc
{

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class UserThrottlingAttribute : ThrottlingLimitRateAttribute
    {
        private readonly long _unauthenticatedCalls;

        private readonly long _unauthenticatedRenewalPeriod;

        public UserThrottlingAttribute(long authenticatedCalls, long authenticatedRenewalPeriod, long unauthenticatedCalls, long unauthenticatedRenewalPeriod)
            : base(authenticatedCalls, authenticatedRenewalPeriod)
        {
            _unauthenticatedCalls = unauthenticatedCalls;
            _unauthenticatedRenewalPeriod = unauthenticatedRenewalPeriod;
        }

        public UserThrottlingAttribute(long authenticatedLimit, long authenticatedWindow)
            : this(authenticatedLimit, authenticatedWindow, 0L, 0L)
        {
        }

        protected override void ApplyCore(ActionModel model, ThrottlingPolicyBuilder builder)
        {
            builder.AddUserLimitRate(Calls, TimeSpan.FromSeconds(RenewalPeriod), _unauthenticatedCalls, TimeSpan.FromSeconds(_unauthenticatedRenewalPeriod), Sliding);
        }
    }
}
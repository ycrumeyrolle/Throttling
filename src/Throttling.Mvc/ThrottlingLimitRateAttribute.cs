using System;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ApplicationModels;

namespace Throttling.Mvc
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public abstract class ThrottlingLimitRateAttribute : ThrottlingConventionAttribute
    {
        protected long Calls { get; }

        protected long RenewalPeriod { get; }

        public ThrottlingLimitRateAttribute(long calls, long renewalPeriod)
        {
            Calls = calls;
            RenewalPeriod = renewalPeriod;
        }

        public bool Sliding { get; set; }

        protected override void ApplyCore(ActionModel model)
        {
            var builder = model.GetOrAddPolicyBuilder(model.AttributeRouteModel.Template);
            ApplyCore(model, builder);
        }

        protected abstract void ApplyCore(ActionModel model, ThrottlingPolicyBuilder builder);
    }
}
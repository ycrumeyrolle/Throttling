using System;
using System.Linq;
using Microsoft.AspNet.Mvc.ApplicationModels;

namespace Throttling.Mvc
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ThrottleAttribute : ThrottleConventionAttribute
    {
        public ThrottleAttribute(string policyName)
        {
            PolicyName = policyName;
        }

        public string PolicyName { get; }

        protected override void ApplyCore(ActionModel model)
        {
            model.Filters.Add(new ThrottleFilterFactory(model.HttpMethods, model.AttributeRouteModel, model.Controller.AttributeRoutes, PolicyName));
        }
    }
}
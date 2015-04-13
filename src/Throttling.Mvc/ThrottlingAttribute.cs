using System;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ApplicationModels;

namespace Throttling.Mvc
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class ThrottlingAttribute : ThrottlingConventionAttribute
    {
        public ThrottlingAttribute(string policyName)
        {
            PolicyName = policyName;
        }

        public string PolicyName { get; }

        protected override void ApplyCore(ActionModel model)
        {
            model.Filters.Add(new ThrottlingAuthorizationFilterFactory(model.HttpMethods, model.AttributeRouteModel.Template, PolicyName));
        }
    }
}
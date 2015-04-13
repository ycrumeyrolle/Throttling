using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ApplicationModels;

namespace Throttling.Mvc
{
    internal static class ActionModelExtensions
    {
        internal static ThrottlingPolicyBuilder GetOrAddPolicyBuilder(this ActionModel model, string policyName)
        {
            // TODO : Add a category based on Url template
            // TODO : Test with areas
            object builderObj;
            ThrottlingPolicyBuilder builder;
            if (!model.Properties.TryGetValue("Throttling.PolicyBuilder", out builderObj))
            {
                builder = new ThrottlingPolicyBuilder(policyName);
                model.Properties.Add("Throttling.PolicyBuilder", builder);
                model.Filters.Add(new ThrottlingAuthorizationFilterFactory(model.HttpMethods, model.AttributeRouteModel.Template, builder));
            }
            else
            {
                builder = (ThrottlingPolicyBuilder)builderObj;
            }

            return builder;
        }
    }
}
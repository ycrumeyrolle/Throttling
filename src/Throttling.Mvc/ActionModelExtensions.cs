using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ApplicationModels;

namespace Throttling.Mvc
{
    internal static class ActionModelExtensions
    {
        internal static ThrottlePolicyBuilder GetOrAddPolicyBuilder(this ActionModel model, string policyName)
        {
            // TODO : Add a category based on Url template
            // TODO : Test with areas
            object builderObj;
            ThrottlePolicyBuilder builder;
            if (!model.Properties.TryGetValue("Throttling.PolicyBuilder", out builderObj))
            {
                builder = new ThrottlePolicyBuilder(policyName);
                model.Properties.Add("Throttling.PolicyBuilder", builder);
                model.Filters.Add(new ThrottleFilterFactory(model.HttpMethods, model.AttributeRouteModel.Template, builder));
            }
            else
            {
                builder = (ThrottlePolicyBuilder)builderObj;
            }

            return builder;
        }
    }
}
namespace Throttling.Mvc
{
    using System;
    using System.Globalization;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc.ApplicationModels;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Mvc.Internal;

    public class ThrottleApplicationModelProvider : IApplicationModelProvider
    {
        public int Order { get { return 20; } }

        public void OnProvidersExecuted(ApplicationModelProviderContext context)
        {
            // Intentionally empty.
        }

        public void OnProvidersExecuting(ApplicationModelProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            foreach (var controllerModel in context.Result.Controllers)
            {
                var controllerEnableThrottling = controllerModel.Attributes.OfType<IEnableThrottlingAttribute>().FirstOrDefault();

                foreach (var actionModel in controllerModel.Actions)
                {
                    if (actionModel.Attributes.OfType<IDisableThrottlingAttribute>().Any())
                    {
                        continue;
                    }

                    IFilterMetadata filter = null;
                    var enableThrottling = actionModel.Attributes.OfType<IEnableThrottlingAttribute>().FirstOrDefault();

                    var controllerRouteModels = controllerModel.Selectors.Select(s => s.AttributeRouteModel);
                    string policyName = null;
                    if (enableThrottling != null)
                    {
                        policyName = enableThrottling.PolicyName;
                    }
                    else if (controllerEnableThrottling != null)
                    {
                        policyName = controllerEnableThrottling.PolicyName;
                    }

                    if (policyName != null)
                    {
                        foreach (var selector in actionModel.Selectors)
                        {
                            var actionRouteModel = selector.AttributeRouteModel;
                            var httpMethods = selector
                                .ActionConstraints
                                .OfType<HttpMethodActionConstraint>()
                                .SelectMany(c => c.HttpMethods)
                                .Distinct();

                            filter = new ThrottleFilterFactory(httpMethods, actionRouteModel, controllerRouteModels, policyName);
                            actionModel.Filters.Add(filter);
                        }
                    }
                }
            }
        }
    }
}
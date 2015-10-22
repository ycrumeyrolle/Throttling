namespace Throttling.Mvc
{
    using System;
    using System.Globalization;
    using System.Linq;
    using Microsoft.AspNet.Mvc.ApplicationModels;
    using Microsoft.AspNet.Mvc.Filters;

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
                    IFilterMetadata filter = null;
                    var enableThrottling = actionModel.Attributes.OfType<IEnableThrottlingAttribute>().FirstOrDefault();
                    if (enableThrottling != null)
                    {
                        filter = new ThrottleFilterFactory(actionModel.HttpMethods, actionModel.AttributeRouteModel, controllerModel.AttributeRoutes, enableThrottling.PolicyName);
                    }
                    else if (controllerEnableThrottling != null)
                    {
                        filter = new ThrottleFilterFactory(actionModel.HttpMethods, actionModel.AttributeRouteModel, controllerModel.AttributeRoutes, controllerEnableThrottling.PolicyName);
                    }

                    var disableThrottling = actionModel.Attributes.OfType<IDisableThrottlingAttribute>().Any();
                    if (!disableThrottling && filter != null)
                    {
                        if (!IsAttributeRoutedAction(actionModel))
                        {
                            throw new InvalidOperationException(string.Format(
                                CultureInfo.CurrentCulture,
                                "The action '{0}.{1}' has throttling enabled, but is using conventional routing. Only actions using attribute routing support Throttling",
                                controllerModel.ControllerName,
                                actionModel.ActionName));
                        }

                        actionModel.Filters.Add(filter);
                    }
                }
            }
        }

        private static bool IsAttributeRoutedAction(ActionModel model)
        {
            return model.AttributeRouteModel?.Template != null;
        }
    }
}
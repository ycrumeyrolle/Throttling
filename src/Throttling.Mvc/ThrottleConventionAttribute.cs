namespace Throttling.Mvc
{
    using System;
    using System.Globalization;
    using Microsoft.AspNet.Mvc.ApplicationModels;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public abstract class ThrottleConventionAttribute : Attribute, IActionModelConvention, IControllerModelConvention, IApplicationModelConvention
    {
        public void Apply(ActionModel model)
        {
            Apply(model, false);
        }

        private void Apply(ActionModel model, bool ignoreConventionalRoutingError)
        {
            if (!ignoreConventionalRoutingError && !IsAttributeRoutedAction(model))
            {
                throw new InvalidOperationException(string.Format(
                    CultureInfo.CurrentCulture,
                    "The action '{0}.{1}' has Throttling enabled, but is using conventional routing. Only actions which use attribute routing support Throttling",
                    model.Controller.ControllerName,
                    model.ActionName));
            }

            ApplyCore(model);
        }

        public void Apply(ControllerModel model)
        {
            foreach (var action in model.Actions)
            {
                Apply(action, false);
            }
        }

        public void Apply(ApplicationModel model)
        {
            foreach (var controller in model.Controllers)
            {
                foreach (var action in controller.Actions)
                {
                    Apply(action, true);
                }
            }
        }

        protected abstract void ApplyCore(ActionModel model);

        private static bool IsAttributeRoutedAction(ActionModel model)
        {
            return model.AttributeRouteModel?.Template != null;
        }
    }
}
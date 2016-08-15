using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.AspNet.Mvc.ApplicationModels;
using Microsoft.AspNet.Mvc.Filters;
using Microsoft.Framework.DependencyInjection;

namespace Throttling.Mvc
{
    /// <summary>
    /// A filter factory which creates a new instance of <see cref="ThrottleFilter"/>.
    /// </summary>
    public class ThrottleFilterFactory : IFilterFactory, IOrderedFilter
    {
        private readonly string _policyName;
        private readonly IEnumerable<string> _httpMethods;
        private readonly ReadOnlyCollection<AttributeRouteModel> _controllerTemplates;
        private readonly AttributeRouteModel _actionTemplate;

        public ThrottleFilterFactory(IList<string> httpMethods, AttributeRouteModel actionTemplate, IEnumerable<AttributeRouteModel> controllerTemplates, string policyName)
        {
            if (httpMethods == null)
            {
                throw new ArgumentNullException(nameof(httpMethods));
            }

            if (actionTemplate == null)
            {
                throw new ArgumentNullException(nameof(actionTemplate));
            }

            if (controllerTemplates == null)
            {
                throw new ArgumentNullException(nameof(controllerTemplates));
            }

            if (policyName == null)
            {
                throw new ArgumentNullException(nameof(policyName));
            }

            _httpMethods = httpMethods;
            _controllerTemplates = new ReadOnlyCollection<AttributeRouteModel>(controllerTemplates.ToArray());
            _actionTemplate = actionTemplate;
            _policyName = policyName;
        }


        /// <inheritdoc />
        public int Order
        {
            get
            {
                return -1;
            }
        }

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            var filter = serviceProvider.GetRequiredService<IThrottleFilter>();
            if (_controllerTemplates.Count == 0)
            {
                filter.Routes = new ThrottleRouteCollection (new[] { CreateRoute(null) });
            }
            else
            {
                filter.Routes = new ThrottleRouteCollection(_controllerTemplates.Select(template => CreateRoute(template)).ToList());
            }
            

            return filter;
        }

        private ThrottleRoute CreateRoute(AttributeRouteModel controllerTemplate)
        {
            string routeTemplate;
            if (controllerTemplate == null)
            {
                routeTemplate = _actionTemplate.Template;
            }
            else
            {                
                routeTemplate = AttributeRouteModel.CombineAttributeRouteModel(controllerTemplate, _actionTemplate).Template;
            }

            return new NamedThrottleRoute(_httpMethods, routeTemplate, _policyName);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ApplicationModels;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Internal;

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

        public IFilterMetadata CreateInstance([NotNull] IServiceProvider serviceProvider)
        {
            var filter = serviceProvider.GetRequiredService<IThrottleFilter>();
            if (_controllerTemplates.Count == 0)
            {
                filter.Routes = new ReadOnlyCollection<ThrottleRoute>(new[] { CreateRoute(null) });
            }
            else
            {
                filter.Routes = new ReadOnlyCollection<ThrottleRoute>(_controllerTemplates.Select(template => CreateRoute(template)).ToList());
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
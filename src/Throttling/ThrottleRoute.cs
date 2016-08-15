using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Routing;
using Microsoft.AspNet.Routing.Template;

namespace Throttling
{
    public abstract class ThrottleRoute
    {
        private static readonly IReadOnlyDictionary<string, object> EmptyRouteValues = new RouteValueDictionary();

        private readonly IEnumerable<string> _httpMethods;

        private readonly TemplateMatcher _matcher;

        public ThrottleRoute(string routeTemplate) 
            : this(routeTemplate, null)
        {
        }

        public ThrottleRoute(string routeTemplate, IEnumerable<string> httpMethods)
        {
            if (routeTemplate == null)
            {
                throw new ArgumentNullException(nameof(routeTemplate));
            }

            _httpMethods = httpMethods;
            RouteTemplate = routeTemplate;
            var route = TemplateParser.Parse(routeTemplate);
            _matcher = new TemplateMatcher(route, EmptyRouteValues);
        }
        
        public string RouteTemplate { get; }        

        public abstract ThrottlePolicy GetPolicy(HttpRequest httpContext, ThrottleOptions options);

        public bool Match(HttpRequest httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            if (_httpMethods == null || _httpMethods.Contains(httpContext.Method))
            {
                var requestPath = httpContext.Path.Value;

                if (!string.IsNullOrEmpty(requestPath) && requestPath[0] == '/')
                {
                    requestPath = requestPath.Substring(1);
                }

                return _matcher.Match(requestPath) != null;
            }

            return false;
        }
    }
}
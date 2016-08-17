using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;

namespace Throttling
{
    public abstract class ThrottleRoute
    {
        private static readonly RouteValueDictionary EmptyRouteValues = new RouteValueDictionary();

#if NET451
        private static readonly string[] EmptyHttpMethods = new string[0];
#else
        private static readonly string[] EmptyHttpMethods = Array.Empty<string>();
#endif

        private readonly HashSet<string> _httpMethods;

        private readonly TemplateMatcher _matcher;

        public ThrottleRoute(string routeTemplate)
            : this(routeTemplate, EmptyHttpMethods)
        {
        }

        public ThrottleRoute(string routeTemplate, IEnumerable<string> httpMethods)
        {
            if (routeTemplate == null)
            {
                throw new ArgumentNullException(nameof(routeTemplate));
            }

            if (httpMethods == null)
            {
                throw new ArgumentNullException(nameof(httpMethods));
            }

            _httpMethods = new HashSet<string>(httpMethods);
            RouteTemplate = routeTemplate;
            var route = TemplateParser.Parse(routeTemplate);
            _matcher = new TemplateMatcher(route, EmptyRouteValues);
        }

        public string RouteTemplate { get; }

        public abstract ThrottlePolicy GetPolicy(HttpRequest httpContext, ThrottleOptions options);

        public bool TryMatch(HttpRequest httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            if (_httpMethods.Count == 0 || _httpMethods.Contains(httpContext.Method))
            {
                var requestPath = httpContext.Path.Value;

                //if (!string.IsNullOrEmpty(requestPath) && requestPath[0] == '/')
                //{
                //    requestPath = requestPath.Substring(1);
                //}

                return _matcher.TryMatch(requestPath, new RouteValueDictionary());
            }

            return false;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Framework.Internal;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Routing;
using Microsoft.AspNet.Routing.Template;

namespace Throttling
{
    public abstract class ThrottlingRoute
    {
        private static readonly IReadOnlyDictionary<string, object> EmptyRouteValues = new RouteValueDictionary();

        private readonly IEnumerable<string> _httpMethods;

        private readonly TemplateMatcher _matcher;

        public ThrottlingRoute(IEnumerable<string> httpMethods, string routeTemplate)
        {
            _httpMethods = httpMethods;
            RouteTemplate = routeTemplate;
            var route = TemplateParser.Parse(routeTemplate);
            _matcher = new TemplateMatcher(route, EmptyRouteValues);
        }

        public ThrottlingRoute(string routeTemplate)
            : this(null, routeTemplate)
        {
        }

        public string RouteTemplate { get; set; }

        public abstract IThrottlingPolicy GetPolicy([NotNull] HttpRequest request, [NotNull] ThrottlingOptions options);

        public bool Match([NotNull] HttpRequest request)
        {
            if (_httpMethods == null || _httpMethods.Contains(request.Method))
            {
                var requestPath = request.Path.Value;

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
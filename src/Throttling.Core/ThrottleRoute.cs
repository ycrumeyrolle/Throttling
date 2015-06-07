using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Routing;
using Microsoft.AspNet.Routing.Template;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public abstract class ThrottleRoute
    {
        private static readonly IReadOnlyDictionary<string, object> EmptyRouteValues = new RouteValueDictionary();

        private readonly IEnumerable<string> _httpMethods;

        private readonly TemplateMatcher _matcher;

        public ThrottleRoute(IEnumerable<string> httpMethods, [NotNull] string routeTemplate)
        {
            _httpMethods = httpMethods;
            RouteTemplate = routeTemplate;
            var route = TemplateParser.Parse(routeTemplate);
            _matcher = new TemplateMatcher(route, EmptyRouteValues);
        }

        public ThrottleRoute(string routeTemplate)
            : this(null, routeTemplate)
        {
        }

        public string RouteTemplate { get; }        

        public abstract ThrottlePolicy GetPolicy([NotNull] HttpRequest httpContext, [NotNull] ThrottleOptions options);

        public bool Match([NotNull] HttpRequest httpContext)
        {
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
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Routing;
using Microsoft.AspNet.Routing.Template;
using Microsoft.Framework.Internal;
using Throttling.IPRanges;

namespace Throttling
{
    public abstract class ThrottlingRoute
    {
        private static readonly IReadOnlyDictionary<string, object> EmptyRouteValues = new RouteValueDictionary();

        private readonly IEnumerable<string> _httpMethods;

        private readonly TemplateMatcher _matcher;

        public ThrottlingRoute(IEnumerable<string> httpMethods, string routeTemplate, IPWhitelist whitelist = null)
        {
            _httpMethods = httpMethods;
            RouteTemplate = routeTemplate;
            var route = TemplateParser.Parse(routeTemplate);
            _matcher = new TemplateMatcher(route, EmptyRouteValues);
            Whitelist = whitelist;
        }

        public ThrottlingRoute(string routeTemplate)
            : this(null, routeTemplate)
        {
        }

        public string RouteTemplate { get; private set; }

        public IPWhitelist Whitelist { get; private set; }

        public abstract ThrottlingPolicy GetPolicy([NotNull] HttpRequest request, [NotNull] ThrottlingOptions options);

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
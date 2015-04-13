using System;
using System.Collections.Generic;
using Microsoft.Framework.Internal;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Routing;
using Microsoft.AspNet.Routing.Template;
using System.Linq;

namespace Throttling
{
    public class RouteKeyProvider : IClientKeyProvider
    {
        private static readonly IReadOnlyDictionary<string, object> EmptyRouteValues = new RouteValueDictionary();

        private readonly string _apiKeyName;
        private TemplateMatcher _matcher;
        private readonly string _routeTemplate;

        public RouteKeyProvider([NotNull] string routeTemplate, [NotNull] string apiKeyName)
        {
            var route = TemplateParser.Parse(routeTemplate);
            if (!route.Parameters.Any(p => p.Name == apiKeyName))
            {
                throw new ArgumentException(string.Format("Unabled to find {0} key in the route template {1}", apiKeyName, routeTemplate), "apiKeyName");
            }

            _routeTemplate = routeTemplate;
            _matcher = new TemplateMatcher(route, EmptyRouteValues);
            _apiKeyName = apiKeyName;
        }

        public string GetKey([NotNull] HttpContext context)
        {
            var requestPath = context.Request.Path.Value;

            if (!string.IsNullOrEmpty(requestPath) && requestPath[0] == '/')
            {
                requestPath = requestPath.Substring(1);
            }

            var match = _matcher.Match(requestPath);
            if (match == null)
            {
                throw new InvalidOperationException("Invalid ");
            }

            return (string)match[_apiKeyName];
        }
    }
}
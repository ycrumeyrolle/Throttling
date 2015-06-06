using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Routing;
using Microsoft.AspNet.Routing.Template;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public class RouteApiKeyProvider : IApiKeyProvider
    {
        private static readonly IReadOnlyDictionary<string, object> EmptyRouteValues = new RouteValueDictionary();

        private readonly string _apiKeyName;
        private TemplateMatcher _matcher;
        private readonly string _routeTemplate;

        public RouteApiKeyProvider([NotNull] string routeTemplate, [NotNull] string apiKeyName)
        {
            var route = TemplateParser.Parse(routeTemplate);
            if (!route.Parameters.Any(p => p.Name == apiKeyName))
            {
                throw new ArgumentException(string.Format("Unabled to find {0} key in the route template {1}", apiKeyName, routeTemplate), nameof(apiKeyName));
            }

            _routeTemplate = routeTemplate;
            _matcher = new TemplateMatcher(route, EmptyRouteValues);
            _apiKeyName = apiKeyName;
        }

        public string GetApiKey([NotNull] HttpContext context)
        {
            var requestPath = context.Request.Path.Value;

            if (!string.IsNullOrEmpty(requestPath) && requestPath[0] == '/')
            {
                requestPath = requestPath.Substring(1);
            }

            var match = _matcher.Match(requestPath);
            if (match == null)
            {
                return null;
            }

            return (string)match[_apiKeyName];
        }
    }
}
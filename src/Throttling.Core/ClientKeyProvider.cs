using System;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Routing;
using Microsoft.AspNet.Routing.Template;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Framework.Internal;
using Microsoft.AspNet.WebUtilities;

namespace Throttling
{
    public class ClientKeyProvider : IClientKeyProvider
    {
        public string GetKey([NotNull] HttpContext context)
        {
            return OnGetKey(context);
        }

        public Func<HttpContext, string> OnGetKey { get; set; } = ctx => "(none)";
    }

    public class QueryStringKeyProvider : IClientKeyProvider
    {
        private readonly string _parameter;

        public QueryStringKeyProvider([NotNull] string parameter)
        {
            _parameter = parameter;
        }

        public string GetKey([NotNull] HttpContext context)
        {
            var keys = QueryHelpers.ParseQuery(context.Request.QueryString.Value);
            string[] apiKeys;
            if (keys != null && keys.TryGetValue(_parameter, out apiKeys))
            {
                if (apiKeys.Length == 0)
                {
                    throw new InvalidOperationException("No API key found in the query string");
                }

                if (apiKeys.Length != 1)
                {
                    throw new InvalidOperationException("Multiples API key found in the query string");
                }

                return apiKeys[0];
            }

            throw new InvalidOperationException("No API key found in the query string");
        }
    }

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
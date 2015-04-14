using System.Collections.Generic;
using Microsoft.Framework.Internal;
using Microsoft.AspNet.Http;
using System;

namespace Throttling
{
    public class ThrottlingRouteCollection : IThrottlingRouter
    {
        private readonly List<ThrottlingRoute> _routes = new List<ThrottlingRoute>();

        public void Add(ThrottlingRoute route)
        {
            _routes.Add(route);
        }

        public ThrottlingStrategy GetThrottlingStrategyAsync([NotNull] HttpContext context, [NotNull] ThrottlingOptions options)
        {
            for (var i = 0; i < _routes.Count; i++)
            {
                var route = _routes[i];

                if (route.Match(context.Request))
                {
                    return new ThrottlingStrategy
                    {
                        Policy = route.GetPolicy(context.Request, options),
                        RouteTemplate = route.RouteTemplate,
                        Whitelist = route.Whitelist
                    };
                }
            }

            return null;
        }

        public int Count { get { return _routes.Count; } }
        
        public IDictionary<string, IThrottlingPolicy> PolicyMap { get; } = new Dictionary<string, IThrottlingPolicy>();
    }
}
using System;
using System.Collections.Generic;
using Microsoft.AspNet.Http;

namespace Throttling
{
    public class ThrottleRouteCollection : IThrottleRouter
    {
        private readonly IReadOnlyList<ThrottleRoute> _routes;

        public ThrottleRouteCollection(IEnumerable<ThrottleRoute> routes)
        {
            _routes = new List<ThrottleRoute>(routes);
        }

        public ThrottleRouteCollection() 
            : this(new List<ThrottleRoute>())
        {
        }
        
        public ThrottleStrategy GetThrottleStrategy(HttpContext httpContext, ThrottleOptions options)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            foreach (var route in _routes)
            {
                if (route.Match(httpContext.Request))
                {
                    return new ThrottleStrategy
                    {
                        Policy = route.GetPolicy(httpContext.Request, options),
                        RouteTemplate = route.RouteTemplate
                    };
                }
            }

            return null;
        }
    }
}
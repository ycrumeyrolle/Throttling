using System.Collections.Generic;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public class ThrottleRouteCollection : IThrottleRouter
    {
        private readonly IReadOnlyList<ThrottleRoute> _routes;

        public ThrottleRouteCollection([NotNull] IEnumerable<ThrottleRoute> routes)
        {
            _routes = new List<ThrottleRoute>(routes);
        }

        public ThrottleRouteCollection() 
            : this(new List<ThrottleRoute>())
        {
        }
        
        public ThrottleStrategy GetThrottleStrategy([NotNull] HttpContext httpContext, [NotNull] ThrottleOptions options)
        {
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
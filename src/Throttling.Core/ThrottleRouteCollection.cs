using System.Collections.Generic;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public class ThrottleRouteCollection : IThrottleRouter
    {
        private readonly List<ThrottleRoute> _routes = new List<ThrottleRoute>();

        public void Add([NotNull] ThrottleRoute route)
        {
            _routes.Add(route);
        }

        public ThrottleStrategy GetThrottleStrategyAsync([NotNull] HttpContext httpContext, [NotNull] ThrottleOptions options)
        {
            for (var i = 0; i < _routes.Count; i++)
            {
                var route = _routes[i];

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

        public int Count
        {
            get
            {
                return _routes.Count;
            }
        }

        public IDictionary<string, ThrottlePolicy> PolicyMap { get; } = new Dictionary<string, ThrottlePolicy>();
    }
}
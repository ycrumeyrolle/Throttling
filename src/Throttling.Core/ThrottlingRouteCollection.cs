using System.Collections.Generic;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public class ThrottlingRouteCollection : IThrottlingRouter
    {
        private readonly List<ThrottlingRoute> _routes = new List<ThrottlingRoute>();

        public void Add([NotNull] ThrottlingRoute route)
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

        public IDictionary<string, ThrottlingPolicy> PolicyMap { get; } = new Dictionary<string, ThrottlingPolicy>();
    }
}
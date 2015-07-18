using System.Collections.Generic;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public class ThrottleRouteCollection : IThrottleRouter
    {
        private readonly List<ThrottleRoute> _routes;
        private readonly IDictionary<string, ThrottlePolicy> _policyMap;

        public ThrottleRouteCollection(List<ThrottleRoute> routes, IDictionary<string, ThrottlePolicy> policyMap)
        {
            _routes = routes;
            _policyMap = policyMap;
        }

        public ThrottleRouteCollection() 
            : this(new List<ThrottleRoute>(), new Dictionary<string, ThrottlePolicy>())
        {
        }
        
        public ThrottleStrategy GetThrottleStrategyAsync([NotNull] HttpContext httpContext, [NotNull] ThrottleOptions options)
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
        
        public IDictionary<string, ThrottlePolicy> PolicyMap { get { return _policyMap; } }
    }
}
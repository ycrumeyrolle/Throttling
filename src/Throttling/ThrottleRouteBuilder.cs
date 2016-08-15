using System;
using System.Collections.Generic;

namespace Throttling
{
    public class ThrottleRouteBuilder : IThrottleRouteBuilder
    {
        private readonly List<ThrottleRoute> _routes = new List<ThrottleRoute>();
        public void Add(ThrottleRoute route)
        {
            if (route == null)
            {
                throw new ArgumentNullException(nameof(route));
            }

            _routes.Add(route);
        }

        public IThrottleRouter Build()
        {
            var router = new ThrottleRouteCollection(_routes);
            return router;
        }
    }
}
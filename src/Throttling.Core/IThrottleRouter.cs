using System.Collections.Generic;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public interface IThrottleRouter
    {
        ThrottleStrategy GetThrottleStrategy([NotNull] HttpContext httpContext, [NotNull] ThrottleOptions options);
    }

    public interface IThrottleRouteBuilder
    {
        void Add([NotNull] ThrottleRoute route);

        IThrottleRouter Build();
    }

    public class ThrottleRouteBuilder : IThrottleRouteBuilder
    {
        private readonly List<ThrottleRoute> _routes = new List<ThrottleRoute>();
                
        public void Add([NotNull] ThrottleRoute route)
        {
            _routes.Add(route);
        }

        public IThrottleRouter Build()
        {
            var router = new ThrottleRouteCollection(_routes);

            return router;
        }
    }
}
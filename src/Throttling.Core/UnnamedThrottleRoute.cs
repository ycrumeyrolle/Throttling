using System.Collections.Generic;
using Microsoft.Framework.Internal;
using Microsoft.AspNet.Http;
using Throttling.IPRanges;

namespace Throttling
{
    public class UnnamedThrottleRoute : ThrottleRoute
    {
        private readonly ThrottlePolicy _policy;

        public UnnamedThrottleRoute(IEnumerable<string> httpMethods, [NotNull] string routeTemplate, ThrottlePolicy policy)
            : base(httpMethods, routeTemplate)
        {
            _policy = policy;
        }

        public UnnamedThrottleRoute(string routeTemplate, [NotNull] ThrottlePolicy policy)
            : base(null, routeTemplate)
        {
            _policy = policy;
        }

        public override ThrottlePolicy GetPolicy([NotNull] HttpRequest httpContext, [NotNull] ThrottleOptions options)
        {
            return _policy;
        }
    }
}
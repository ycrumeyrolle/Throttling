using System.Collections.Generic;
using Microsoft.Framework.Internal;
using Microsoft.AspNet.Http;
using Throttling.IPRanges;

namespace Throttling
{
    public class UnnamedThrottlingRoute : ThrottlingRoute
    {
        private readonly ThrottlingPolicy _policy;

        public UnnamedThrottlingRoute(IEnumerable<string> httpMethods, [NotNull] string routeTemplate, ThrottlingPolicy policy)
            : base(httpMethods, routeTemplate)
        {
            _policy = policy;
        }

        public UnnamedThrottlingRoute(string routeTemplate, [NotNull] ThrottlingPolicy policy)
            : base(null, routeTemplate)
        {
            _policy = policy;
        }

        public override ThrottlingPolicy GetPolicy([NotNull] HttpRequest request, [NotNull] ThrottlingOptions options)
        {
            return _policy;
        }
    }
}
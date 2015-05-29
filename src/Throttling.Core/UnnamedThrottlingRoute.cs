using System.Collections.Generic;
using Microsoft.Framework.Internal;
using Microsoft.AspNet.Http;
using Throttling.IPRanges;

namespace Throttling
{
    public class UnnamedThrottlingRoute : ThrottlingRoute
    {
        private readonly ThrottlingPolicy _policy;

        public UnnamedThrottlingRoute(IEnumerable<string> httpMethods, [NotNull] string routeTemplate, ThrottlingPolicy policy, IPWhitelist whitelist = null)
            : base(httpMethods, routeTemplate, whitelist)
        {
            _policy = policy;
        }

        public UnnamedThrottlingRoute(string routeTemplate, [NotNull] ThrottlingPolicy policy, IPWhitelist whitelist = null)
            : base(null, routeTemplate, whitelist)
        {
            _policy = policy;
        }

        public override ThrottlingPolicy GetPolicy([NotNull] HttpRequest request, [NotNull] ThrottlingOptions options)
        {
            return _policy;
        }
    }
}
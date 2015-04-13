using System.Collections.Generic;
using Microsoft.Framework.Internal;
using Microsoft.AspNet.Http;

namespace Throttling
{
    public class UnnamedThrottlingRoute : ThrottlingRoute
    {
        private readonly IThrottlingPolicy _policy;

        public UnnamedThrottlingRoute(IEnumerable<string> httpMethods, string routeTemplate, IThrottlingPolicy policy)
            : base(httpMethods, routeTemplate)
        {
            _policy = policy;
        }

        public UnnamedThrottlingRoute(string routeTemplate, IThrottlingPolicy policy)
            : base(null, routeTemplate)
        {
            _policy = policy;
        }

        public override IThrottlingPolicy GetPolicy([NotNull] HttpRequest request, [NotNull] ThrottlingOptions options)
        {
            return _policy;
        }
    }
}
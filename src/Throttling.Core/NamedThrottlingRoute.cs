using System.Collections.Generic;
using Microsoft.Framework.Internal;
using Microsoft.AspNet.Http;
using Throttling.IPRanges;

namespace Throttling
{
    public class NamedThrottlingRoute : ThrottlingRoute
    {
        private readonly string _policyName;

        public NamedThrottlingRoute(IEnumerable<string> httpMethods, string routeTemplate, [NotNull] string policyName)
            : base(httpMethods, routeTemplate)
        {
            _policyName = policyName;
        }

        public NamedThrottlingRoute(string routeTemplate, string policyName)
            : base(null, routeTemplate)
        {
            _policyName = policyName;
        }

        public override ThrottlingPolicy GetPolicy([NotNull] HttpRequest httpContext, [NotNull] ThrottlingOptions options)
        {
            return options.GetPolicy(_policyName);
        }
    }
}
using System.Collections.Generic;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public class NamedThrottleRoute : ThrottleRoute
    {
        private readonly string _policyName;

        public NamedThrottleRoute(IEnumerable<string> httpMethods, string routeTemplate, [NotNull] string policyName)
            : base(httpMethods, routeTemplate)
        {
            _policyName = policyName;
        }

        public NamedThrottleRoute(string routeTemplate, string policyName)
            : base(null, routeTemplate)
        {
            _policyName = policyName;
        }

        public override ThrottlePolicy GetPolicy([NotNull] HttpRequest httpContext, [NotNull] ThrottleOptions options)
        {
            return options.GetPolicy(_policyName);
        }
    }
}
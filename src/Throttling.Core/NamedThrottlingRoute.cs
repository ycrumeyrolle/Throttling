using System.Collections.Generic;
using Microsoft.Framework.Internal;
using Microsoft.AspNet.Http;
using Throttling.IPRanges;

namespace Throttling
{
    public class NamedThrottlingRoute : ThrottlingRoute
    {
        private readonly string _policyName;

        public NamedThrottlingRoute(IEnumerable<string> httpMethods, string routeTemplate, [NotNull] string policyName, IPWhitelist whitelist = null)
            : base(httpMethods, routeTemplate, whitelist)
        {
            _policyName = policyName;
        }

        public NamedThrottlingRoute(string routeTemplate, string policyName, IPWhitelist whitelist = null)
            : base(null, routeTemplate, whitelist)
        {
            _policyName = policyName;
        }

        public override ThrottlingPolicy GetPolicy([NotNull] HttpRequest request, [NotNull] ThrottlingOptions options)
        {
            return options.GetPolicy(_policyName);
        }
    }
}
using System.Collections.Generic;
using Microsoft.Framework.Internal;
using Microsoft.AspNet.Http;

namespace Throttling
{
    public class NamedThrottlingRoute : ThrottlingRoute
    {
        private readonly string _policyName;

        public NamedThrottlingRoute(IEnumerable<string> httpMethods, string routeTemplate, string policyName)
            : base(httpMethods, routeTemplate)
        {
            _policyName = policyName;
        }

        public NamedThrottlingRoute(string routeTemplate, string policyName)
            : base(null, routeTemplate)
        {
            _policyName = policyName;
        }

        public override IThrottlingPolicy GetPolicy([NotNull] HttpRequest request, [NotNull] ThrottlingOptions options)
        {
            return options.GetPolicy(_policyName);
        }
    }
}
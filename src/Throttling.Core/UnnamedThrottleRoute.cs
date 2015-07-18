using System.Collections.Generic;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public class UnnamedThrottleRoute : ThrottleRoute
    {
        private readonly IThrottlePolicyBuilder _policyBuilder;

        private ThrottlePolicy _policy;

        public UnnamedThrottleRoute(IEnumerable<string> httpMethods, [NotNull] string routeTemplate, [NotNull]  IThrottlePolicyBuilder policyBuilder)
            : base(httpMethods, routeTemplate)
        {
            _policyBuilder = policyBuilder;
        }

        public UnnamedThrottleRoute(string routeTemplate, [NotNull] IThrottlePolicyBuilder policyBuilder)
            : base(null, routeTemplate)
        {
            _policyBuilder = policyBuilder;
        }

        public override ThrottlePolicy GetPolicy([NotNull] HttpRequest httpContext, [NotNull] ThrottleOptions options)
        {
            if (_policy == null)
            {
                _policy = _policyBuilder.Build(options);
            }

            return _policy;
        }
    }
}
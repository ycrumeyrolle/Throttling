using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Throttling
{
    public class UnnamedThrottleRoute : ThrottleRoute
    {
        private readonly IThrottlePolicyBuilder _policyBuilder;

        private ThrottlePolicy _policy;

        public UnnamedThrottleRoute(IEnumerable<string> httpMethods, string routeTemplate, IThrottlePolicyBuilder policyBuilder)
            : base(routeTemplate, httpMethods)
        {
            if (policyBuilder == null)
            {
                throw new ArgumentNullException(nameof(policyBuilder));
            }

            _policyBuilder = policyBuilder;
        }

        public UnnamedThrottleRoute(IEnumerable<string> httpMethods, string routeTemplate, ThrottlePolicy policy)
            : base(routeTemplate, httpMethods)
        {
            if (policy == null)
            {
                throw new ArgumentNullException(nameof(policy));
            }

            _policy = policy;
        }

        public UnnamedThrottleRoute(string routeTemplate, ThrottlePolicy policy)
            : base(routeTemplate)
        {
            if (policy == null)
            {
                throw new ArgumentNullException(nameof(policy));
            }

            _policy = policy;
        }

        public override ThrottlePolicy GetPolicy(HttpRequest httpContext, ThrottleOptions options)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (_policy == null)
            {
                _policy = _policyBuilder.Build(options);
            }

            return _policy;
        }
    }
}
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Internal;

namespace Throttling
{
    public class NamedThrottleRoute : ThrottleRoute
    {
        private readonly string _policyName;

        public NamedThrottleRoute(IEnumerable<string> httpMethods, string routeTemplate, string policyName)
            : base(routeTemplate, httpMethods)
        {
            if (policyName == null)
            {
                throw new ArgumentNullException(nameof(policyName));
            }

            _policyName = policyName;
        }

        public NamedThrottleRoute(string routeTemplate, string policyName)
            : base(routeTemplate)
        {
            _policyName = policyName;
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

            return options.GetPolicy(_policyName);
        }
    }
}
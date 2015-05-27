using System;
using System.Collections.Generic;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Internal;

namespace Throttling.Mvc
{
    /// <summary>
    /// A filter factory which creates a new instance of <see cref="ThrottlingAuthorizationFilter"/>.
    /// </summary>
    public class ThrottlingAuthorizationFilterFactory : IFilterFactory, IOrderedFilter
    {
        private readonly string _policyName;

        private readonly ThrottlingPolicyBuilder _builder;
        private readonly string _routeTemplate;
        private readonly IEnumerable<string> _httpMethods;
        
        /// <summary>
        /// Creates a new instance of <see cref="ThrottlingAuthorizationFilterFactory"/>.
        /// </summary>
        /// <param name="policyName"></param>
        public ThrottlingAuthorizationFilterFactory(IEnumerable<string> httpMethods, string routeTemplate, string policyName)
        {
            _policyName = policyName;
            _httpMethods = httpMethods;
            _routeTemplate = routeTemplate;
        }

        /// <summary>
        /// Creates a new instance of <see cref="ThrottlingAuthorizationFilterFactory"/>.
        /// </summary>
        /// <param name="policy"></param>
        public ThrottlingAuthorizationFilterFactory(IEnumerable<string> httpMethods, string routeTemplate, ThrottlingPolicyBuilder builder)
        {
            _builder = builder;
            _httpMethods = httpMethods;
            _routeTemplate = routeTemplate;
        }

        /// <inheritdoc />
        public int Order
        {
            get
            {
                return -1;
            }
        }

        public IFilter CreateInstance([NotNull] IServiceProvider serviceProvider)
        {
            var filter = serviceProvider.GetRequiredService<IThrottlingAuthorizationFilter>();
            if (_policyName == null)
            {
                filter.Route = new UnnamedThrottlingRoute(_httpMethods, _routeTemplate, _builder.Build());
            }
            else
            {
                filter.Route = new NamedThrottlingRoute(_httpMethods, _routeTemplate, _policyName);
            }

            return filter;
        }
    }
}
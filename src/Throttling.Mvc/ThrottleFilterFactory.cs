using System;
using System.Collections.Generic;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Internal;

namespace Throttling.Mvc
{
    /// <summary>
    /// A filter factory which creates a new instance of <see cref="ThrottleFilter"/>.
    /// </summary>
    public class ThrottleFilterFactory : IFilterFactory, IOrderedFilter
    {
        private readonly string _policyName;
        private readonly ThrottlePolicyBuilder _builder;
        private readonly string _routeTemplate;
        private readonly IEnumerable<string> _httpMethods;
        
        /// <summary>
        /// Creates a new instance of <see cref="ThrottleFilterFactory"/>.
        /// </summary>
        /// <param name="policyName"></param>
        public ThrottleFilterFactory(IEnumerable<string> httpMethods, string routeTemplate, string policyName)
        {
            _policyName = policyName;
            _httpMethods = httpMethods;
            _routeTemplate = routeTemplate;
        }

        /// <summary>
        /// Creates a new instance of <see cref="ThrottleFilterFactory"/>.
        /// </summary>
        /// <param name="policy"></param>
        public ThrottleFilterFactory(IEnumerable<string> httpMethods, string routeTemplate, ThrottlePolicyBuilder builder)
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
            var filter = serviceProvider.GetRequiredService<IThrottleFilter>();
            if (_policyName == null)
            {
                filter.Route = new UnnamedThrottleRoute(_httpMethods, _routeTemplate, _builder.Build());
            }
            else
            {
                filter.Route = new NamedThrottleRoute(_httpMethods, _routeTemplate, _policyName);
            }

            return filter;
        }
    }
}
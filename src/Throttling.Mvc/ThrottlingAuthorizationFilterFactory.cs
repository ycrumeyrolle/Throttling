using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.WebUtilities;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Internal;
using Throttling;

namespace Throttling.Mvc
{
    /// <summary>
    /// A filter factory which creates a new instance of <see cref="ThrottlingAuthorizationFilter"/>.
    /// </summary>
    public class ThrottlingAuthorizationFilterFactory : IFilterFactory, IOrderedFilter
    {
        private readonly string _policyName;

        /// <summary>
        /// Creates a new insntace of <see cref="ThrottlingAuthorizationFilterFactory"/>.
        /// </summary>
        /// <param name="policyName"></param>
        public ThrottlingAuthorizationFilterFactory(string policyName)
        {
            _policyName = policyName;
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
            filter.PolicyName = _policyName;
            return filter;
        }
    }
}
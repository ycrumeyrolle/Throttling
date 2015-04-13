using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.Framework.OptionsModel;

namespace Throttling
{
    public class DefaultThrottlingPolicyProvider : IThrottlingPolicyProvider
    {
        private readonly ThrottlingOptions _options;

        /// <summary>
        /// Creates a new instance of <see cref="DefaultThrottlingPolicyProvider"/>.
        /// </summary>
        /// <param name="options">The options configured for the application.</param>
        public DefaultThrottlingPolicyProvider(IOptions<ThrottlingOptions> options)
        {
            _options = options.Options;
        }

        /// <inheritdoc />
        public virtual Task<ThrottlingStrategy> GetThrottlingStrategyAsync(HttpContext context, string policyName)
        {
            IThrottlingPolicy policy;
            if (policyName != null)
            {
                policy = _options.GetPolicy(policyName);
                if (policy != null)
                {
                    return Task.FromResult(new ThrottlingStrategy { Policy = policy, RouteTemplate = "*" });
                }
            }

            ThrottlingStrategy strategy = _options.Routes.GetThrottlingStrategyAsync(context, _options);

            return Task.FromResult(strategy);
        }
    }
}
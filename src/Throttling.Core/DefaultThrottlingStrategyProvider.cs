using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;
using Microsoft.Framework.OptionsModel;

namespace Throttling
{
    public class DefaultThrottleStrategyProvider : IThrottleStrategyProvider
    {
        private readonly ThrottleOptions _options;

        /// <summary>
        /// Creates a new instance of <see cref="DefaultThrottleStrategyProvider"/>.
        /// </summary>
        /// <param name="options">The options configured for the application.</param>
        public DefaultThrottleStrategyProvider([NotNull] IOptions<ThrottleOptions> options)
        {
            _options = options.Options;
        }

        /// <inheritdoc />
        public virtual Task<ThrottleStrategy> GetThrottleStrategyAsync([NotNull] HttpContext httpContext, string policyName, IThrottleRouter fallbackRouter)
        {
            ThrottlePolicy policy;
            if (policyName != null)
            {
                policy = _options.GetPolicy(policyName);
                if (policy != null)
                {
                    return Task.FromResult(new ThrottleStrategy { Policy = policy, RouteTemplate = "{*any}" });
                }
            }

            ThrottleStrategy strategy = _options.Routes.GetThrottleStrategy(httpContext, _options);
                        
            if (strategy == null)
            {
                strategy = fallbackRouter?.GetThrottleStrategy(httpContext, _options);
            }

            return Task.FromResult(strategy);
        }
    }
}
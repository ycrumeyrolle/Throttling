using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Throttling
{
    public class DefaultThrottleStrategyProvider : IThrottleStrategyProvider
    {
        private readonly ThrottleOptions _options;

        /// <summary>
        /// Creates a new instance of <see cref="DefaultThrottleStrategyProvider"/>.
        /// </summary>
        /// <param name="options">The options configured for the application.</param>
        public DefaultThrottleStrategyProvider(IOptions<ThrottleOptions> options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _options = options.Value;
        }

        /// <inheritdoc />
        public virtual Task<ThrottleStrategy> GetThrottleStrategyAsync(HttpContext httpContext, string policyName, IThrottleRouter fallbackRouter)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

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
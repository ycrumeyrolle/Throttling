using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;
using Microsoft.Framework.OptionsModel;

namespace Throttling
{
    public class DefaultThrottlingStrategyProvider : IThrottlingStrategyProvider
    {
        private readonly ThrottlingOptions _options;

        /// <summary>
        /// Creates a new instance of <see cref="DefaultThrottlingStrategyProvider"/>.
        /// </summary>
        /// <param name="options">The options configured for the application.</param>
        public DefaultThrottlingStrategyProvider([NotNull] IOptions<ThrottlingOptions> options)
        {
            _options = options.Options;
        }

        /// <inheritdoc />
        public virtual Task<ThrottlingStrategy> GetThrottlingStrategyAsync([NotNull] HttpContext context, string policyName)
        {
            ThrottlingPolicy policy;
            if (policyName != null)
            {
                policy = _options.GetPolicy(policyName);
                if (policy != null)
                {
                    return Task.FromResult(new ThrottlingStrategy { Policy = policy, RouteTemplate = "{*any}" });
                }
            }

            ThrottlingStrategy strategy = _options.Routes.GetThrottlingStrategyAsync(context, _options);

            return Task.FromResult(strategy);
        }
    }
}
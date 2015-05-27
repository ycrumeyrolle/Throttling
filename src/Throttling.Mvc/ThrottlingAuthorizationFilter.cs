using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Internal;
using Microsoft.Framework.OptionsModel;

namespace Throttling.Mvc
{
    /// <summary>
    /// A filter which applies the given <see cref="ThrottlingPolicy"/> and adds appropriate response headers.
    /// </summary>
    public class ThrottlingAuthorizationFilter : IThrottlingAuthorizationFilter
    {
        private readonly IThrottlingService _throttlingService;
        private readonly IThrottlingPolicyProvider _throttlingPolicyProvider;
        private readonly ThrottlingOptions _options;
        private readonly ISystemClock _clock;


        /// <summary>
        /// Creates a new instace of <see cref="ThrottlingAuthorizationFilter"/>.
        /// </summary>
        /// <param name="ThrottlingService">The <see cref="IThrottlingService"/>.</param>
        /// <param name="policyProvider">The <see cref="IThrottlingPolicyProvider"/>.</param>
        public ThrottlingAuthorizationFilter(IOptions<ThrottlingOptions> optionsAccessor, IThrottlingService ThrottlingService, IThrottlingPolicyProvider policyProvider, ISystemClock clock)
        {
            _throttlingService = ThrottlingService;
            _throttlingPolicyProvider = policyProvider;
            _options = optionsAccessor.Options;
            _clock = clock;
        }

        /// <inheritdoc />
        public int Order
        {
            get
            {
                return -1;
            }
        }

        public string PolicyName { get; set; }

        public ThrottlingRoute Route { get; set; }

        /// <inheritdoc />
        public async Task OnAuthorizationAsync([NotNull] AuthorizationContext context)
        {
            var httpContext = context.HttpContext;
            var request = httpContext.Request;

            var strategy = await _throttlingPolicyProvider?.GetThrottlingStrategyAsync(httpContext, PolicyName);
            if (strategy == null && Route.Match(request))
            {
                strategy = new ThrottlingStrategy
                {
                    Policy = Route.GetPolicy(httpContext.Request, _options),
                    RouteTemplate = Route.RouteTemplate
                };
            }

            if (strategy != null)
            {
                var throttlingContext = await _throttlingService.EvaluateAsync(httpContext, strategy);
                foreach (var header in throttlingContext.Headers.OrderBy(h => h.Key))
                {
                    context.HttpContext.Response.Headers.SetValues(header.Key, header.Value);
                }

                if (throttlingContext.HasFailed)
                {
                    string retryAfter = RetryAfterHelper.GetRetryAfterValue(_clock, _options.RetryAfterMode, throttlingContext.RetryAfter);
                    context.Result = new TooManyRequestResult(throttlingContext.Headers, retryAfter);
                }
            }
        }
    }
}

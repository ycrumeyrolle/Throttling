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
    public class ThrottlingFilter : IThrottlingFilter
    {
        private static readonly object ThrottlingContextKey = new object();

        private readonly IThrottlingService _throttlingService;
        private readonly IThrottlingStrategyProvider _throttlingPolicyProvider;
        private readonly ThrottlingOptions _options;
        private readonly ISystemClock _clock;

        /// <summary>
        /// Creates a new instace of <see cref="ThrottlingFilter"/>.
        /// </summary>
        /// <param name="ThrottlingService">The <see cref="IThrottlingService"/>.</param>
        /// <param name="policyProvider">The <see cref="IThrottlingStrategyProvider"/>.</param>
        public ThrottlingFilter(IOptions<ThrottlingOptions> optionsAccessor, IThrottlingService ThrottlingService, IThrottlingStrategyProvider policyProvider, ISystemClock clock)
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

            var strategy = await _throttlingPolicyProvider?.GetThrottlingStrategyAsync(httpContext, PolicyName);
            if (strategy == null && Route.Match(httpContext.Request))
            {
                strategy = new ThrottlingStrategy
                {
                    Policy = Route.GetPolicy(httpContext.Request, _options),
                    RouteTemplate = Route.RouteTemplate
                };
            }

            if (strategy == null)
            {
                return;
            }

            var throttlingContext = await _throttlingService.EvaluateAsync(httpContext, strategy);
            if (throttlingContext.HasAborted)
            {
                return;
            }

            if (_options.SendThrottlingHeaders)
            {
                foreach (var header in throttlingContext.Headers.OrderBy(h => h.Key))
                {
                    httpContext.Response.Headers.SetValues(header.Key, header.Value);
                }
            }

            if (throttlingContext.HasTooManyRequest)
            {
                string retryAfter = RetryAfterHelper.GetRetryAfterValue(_clock, _options.RetryAfterMode, throttlingContext.RetryAfter);
                context.Result = new TooManyRequestResult(throttlingContext.Headers, retryAfter);
            }

            context.HttpContext.Items[ThrottlingContextKey] = throttlingContext;
        }

        public async Task OnResultExecutionAsync([NotNull]ResultExecutingContext context, [NotNull]ResultExecutionDelegate next)
        {
            if (!context.Cancel)
            {
                var resultExecutedContext = await next();
                var throttlingContext = (ThrottlingContext)resultExecutedContext.HttpContext.Items[ThrottlingContextKey];

                await _throttlingService.PostEvaluateAsync(throttlingContext);
            }
        }
    }
}

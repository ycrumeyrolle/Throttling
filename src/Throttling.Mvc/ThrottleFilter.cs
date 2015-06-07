using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Internal;
using Microsoft.Framework.OptionsModel;

namespace Throttling.Mvc
{
    /// <summary>
    /// A filter which applies the given <see cref="ThrottlePolicy"/> and adds appropriate response headers.
    /// </summary>
    public class ThrottleFilter : IThrottleFilter
    {
        private static readonly object ThrottleContextKey = new object();

        private readonly IThrottleService _throttleService;
        private readonly IThrottleStrategyProvider _strategyProvider;
        private readonly ThrottleOptions _options;
        private readonly ISystemClock _clock;

        /// <summary>
        /// Creates a new instace of <see cref="ThrottleFilter"/>.
        /// </summary>
        /// <param name="throttleService">The <see cref="IThrottleService"/>.</param>
        /// <param name="strategyProvider">The <see cref="IThrottleStrategyProvider"/>.</param>
        public ThrottleFilter(IOptions<ThrottleOptions> optionsAccessor, IThrottleService throttleService, IThrottleStrategyProvider strategyProvider, ISystemClock clock)
        {
            _throttleService = throttleService;
            _strategyProvider = strategyProvider;
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

        public ThrottleRoute Route { get; set; }

        /// <inheritdoc />
        public async Task OnAuthorizationAsync([NotNull] AuthorizationContext context)
        {
            var httpContext = context.HttpContext;

            var strategy = await _strategyProvider?.GetThrottleStrategyAsync(httpContext, PolicyName);
            if (strategy == null && Route.Match(httpContext.Request))
            {
                strategy = new ThrottleStrategy
                {
                    Policy = Route.GetPolicy(httpContext.Request, _options),
                    RouteTemplate = Route.RouteTemplate
                };
            }

            if (strategy == null)
            {
                return;
            }

            var throttleContext = await _throttleService.EvaluateAsync(httpContext, strategy);
            if (throttleContext.HasAborted)
            {
                return;
            }

            if (_options.SendThrottleHeaders)
            {
                foreach (var header in throttleContext.Headers.OrderBy(h => h.Key))
                {
                    httpContext.Response.Headers.SetValues(header.Key, header.Value);
                }
            }

            if (throttleContext.HasTooManyRequest)
            {
                string retryAfter = RetryAfterHelper.GetRetryAfterValue(_clock, _options.RetryAfterMode, throttleContext.RetryAfter);
                context.Result = new TooManyRequestResult(throttleContext.Headers, retryAfter);
            }

            context.HttpContext.Items[ThrottleContextKey] = throttleContext;
        }

        public async Task OnResultExecutionAsync([NotNull]ResultExecutingContext context, [NotNull]ResultExecutionDelegate next)
        {
            if (!context.Cancel)
            {
                var resultExecutedContext = await next();
                var throttleContext = (ThrottleContext)resultExecutedContext.HttpContext.Items[ThrottleContextKey];

                await _throttleService.PostEvaluateAsync(throttleContext);
            }
        }
    }
}

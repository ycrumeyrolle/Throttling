using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;
using Microsoft.Framework.Logging;
using Microsoft.Framework.OptionsModel;

namespace Throttling
{
    /// <summary>
    /// An ASP.NET middleware for handling Throttling.
    /// </summary>
    public class ThrottlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IThrottlingService _throttlingService;
        private readonly IThrottlingStrategyProvider _throttlingPolicyProvider;
        private readonly ILogger _logger;
        private readonly ISystemClock _clock;
        private readonly ThrottlingOptions _options;

        /// <summary>
        /// Instantiates a new <see cref="T:Throttling.ThrottlingMiddleware" />.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <param name="throttlingService">An instance of <see cref="T:Throttling.IThrottlingService" />.</param>
        /// <param name="policy">An instance of the <see cref="T:Throttling.ThrottlingPolicy" /> which can be applied.</param>
        public ThrottlingMiddleware(
            [NotNull] RequestDelegate next,
            [NotNull] ILoggerFactory loggerFactory,
            [NotNull] IThrottlingService throttlingService,
            [NotNull] IThrottlingStrategyProvider policyProvider,
            [NotNull] ISystemClock clock,
            [NotNull] IOptions<ThrottlingOptions> options)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<ThrottlingMiddleware>();
            _throttlingService = throttlingService;
            _throttlingPolicyProvider = policyProvider;
            _clock = clock;
            _options = options.Options;
        }

        /// <inheritdoc />
        public async Task Invoke(HttpContext context)
        {
            var strategy = await _throttlingPolicyProvider?.GetThrottlingStrategyAsync(context, null);
            if (strategy == null)
            {
                _logger.LogVerbose("No strategy for current request.");
                await _next(context);
                return;
            }

            var throttlingContext = await _throttlingService.EvaluateAsync(context, strategy);
            var response = context.Response;
            if (_options.SendThrottlingHeaders)
            {
                foreach (var header in throttlingContext.Headers.OrderBy(h => h.Key))
                {
                    response.Headers.SetValues(header.Key, header.Value);
                }
            }

            if (throttlingContext.HasTooManyRequest)
            {
                _logger.LogInformation("Throttling applied.");
                string retryAfter = RetryAfterHelper.GetRetryAfterValue(_clock, _options.RetryAfterMode, throttlingContext.RetryAfter);

                response.StatusCode = Constants.Status429TooManyRequests;

                // rfc6585 section 4 : Responses with the 429 status code MUST NOT be stored by a cache.
                response.Headers.SetValues("Cache-Control", "no-store", "no-cache");
                response.Headers.Set("Pragma", "no-cache");

                // rfc6585 section 4 : The response [...] MAY include a Retry-After header indicating how long to wait before making a new request.
                if (retryAfter != null)
                {
                    response.Headers.Set("Retry-After", retryAfter);
                }
            }
            else
            {
                _logger.LogInformation("No throttling applied.");
                await _next(context);
            }

            await _throttlingService.PostEvaluateAsync(throttlingContext);
        }
    }
}

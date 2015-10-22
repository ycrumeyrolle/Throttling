using System;
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
    public class ThrottleMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IThrottleService _throttleService;
        private readonly IThrottleStrategyProvider _strategyProvider;
        private readonly ILogger _logger;
        private readonly ISystemClock _clock;
        private readonly ThrottleOptions _options;

        /// <summary>
        /// Instantiates a new <see cref="T:ThrottleMiddleware" />.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <param name="throttleService">An instance of <see cref="T:IThrottleService" />.</param>
        /// <param name="policy">An instance of the <see cref="T:ThrottlePolicy" /> which can be applied.</param>
        public ThrottleMiddleware(
           RequestDelegate next,
           ILoggerFactory loggerFactory,
           IThrottleService throttleService,
           IThrottleStrategyProvider strategyProvider,
           ISystemClock clock,
           IOptions<ThrottleOptions> options,
            ConfigureOptions<ThrottleOptions> configureOptions)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            if (throttleService == null)
            {
                throw new ArgumentNullException(nameof(throttleService));
            }

            if (strategyProvider == null)
            {
                throw new ArgumentNullException(nameof(strategyProvider));
            }

            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            if (clock == null)
            {
                throw new ArgumentNullException(nameof(clock));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _next = next;
            _logger = loggerFactory.CreateLogger<ThrottleMiddleware>();
            _throttleService = throttleService;
            _strategyProvider = strategyProvider;
            _clock = clock;
            _options = options.Value;

            if (configureOptions != null)
            {
                configureOptions.Configure(_options);
            }
        }

        /// <inheritdoc />
        public async Task Invoke(HttpContext httpContext)
        {
            var strategy = await _strategyProvider?.GetThrottleStrategyAsync(httpContext, null, null);
            if (strategy == null)
            {
                _logger.LogVerbose("No strategy for current request.");
                await _next(httpContext);
                return;
            }

            var throttleContext = await _throttleService.EvaluateAsync(httpContext, strategy);
            if (throttleContext.HasAborted)
            {
                _logger.LogVerbose("Throttling aborted. No throttling applied.");
                await _next(httpContext);
                return;
            }

            var response = httpContext.Response;
            if (_options.SendThrottleHeaders)
            {
                foreach (var header in throttleContext.ResponseHeaders.OrderBy(h => h.Key))
                {
                    response.Headers[header.Key] = header.Value;
                }
            }

            if (throttleContext.HasTooManyRequest)
            {
                _logger.LogInformation("Throttling applied.");
                string retryAfter = RetryAfterHelper.GetRetryAfterValue(_clock, _options.RetryAfterMode, throttleContext.RetryAfter);

                response.StatusCode = Constants.Status429TooManyRequests;

                // rfc6585 section 4 : Responses with the 429 status code MUST NOT be stored by a cache.
                response.Headers.SetCommaSeparatedValues("Cache-Control", "no-store", "no-cache");
                response.Headers["Pragma"] = "no-cache";

                // rfc6585 section 4 : The response [...] MAY include a Retry-After header indicating how long to wait before making a new request.
                if (retryAfter != null)
                {
                    response.Headers["Retry-After"] = retryAfter;
                }
            }
            else
            {
                _logger.LogVerbose("No throttling applied.");
                await _next(httpContext);
            }

            await _throttleService.PostEvaluateAsync(throttleContext);
        }
    }
}

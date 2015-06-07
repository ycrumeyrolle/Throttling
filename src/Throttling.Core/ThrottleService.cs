using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;
using Microsoft.Framework.Logging;
using Microsoft.Framework.OptionsModel;

namespace Throttling
{
    public class ThrottleService : IThrottleService
    {
        private readonly ThrottleOptions _options;
        private readonly ILogger _logger;
        private readonly ISystemClock _clock;
        private readonly IList<IThrottleHandler> _handlers;
        private readonly IList<IExclusionHandler> _exclusionHandlers;

        public ThrottleService(
            [NotNull] ILoggerFactory loggerFactory,
            [NotNull] IEnumerable<IThrottleHandler> handlers,
            [NotNull] IEnumerable<IExclusionHandler> exclusionHandlers,
            [NotNull] ISystemClock clock,
            [NotNull] IOptions<ThrottleOptions> options,
            ConfigureOptions<ThrottleOptions> configureOptions = null)
        {
            if (configureOptions != null)
            {
                _options = options.GetNamedOptions(configureOptions.Name);
                configureOptions.Configure(_options, configureOptions.Name);
            }
            else
            {
                _options = options.Options;
            }

            _handlers = handlers.ToArray();
            _exclusionHandlers = exclusionHandlers.ToArray();
            _logger = loggerFactory.CreateLogger<ThrottleService>();
            _clock = clock;
        }

        public virtual async Task<ThrottleContext> EvaluateAsync([NotNull] HttpContext httpContext, [NotNull] ThrottleStrategy strategy)
        {
            var throttleContext = new ThrottleContext(httpContext, strategy);

            for (int i = 0; i < _exclusionHandlers.Count; i++)
            {
                await _exclusionHandlers[i].HandleAsync(throttleContext);
            }

            if (!throttleContext.HasAborted)
            {
                for (int i = 0; i < _handlers.Count; i++)
                {
                    await _handlers[i].HandleAsync(throttleContext);
                }
            }

            return throttleContext;
        }

        private string GetRetryAfterValue(RetryAfterMode mode, DateTimeOffset reset)
        {
            switch (mode)
            {
                case RetryAfterMode.HttpDate:
                    return reset.ToString("r");
                case RetryAfterMode.DeltaSeconds:
                    return Convert.ToInt64((reset - _clock.UtcNow).TotalSeconds).ToString(CultureInfo.InvariantCulture);
                default:
                    return null;
            }
        }

        public async Task PostEvaluateAsync([NotNull] ThrottleContext throttleContext)
        {
            for (int i = 0; i < _handlers.Count; i++)
            {
                await _handlers[i].PostHandleAsync(throttleContext);
            }
        }
    }
}
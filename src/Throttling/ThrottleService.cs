using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Throttling
{
    public class ThrottleService : IThrottleService
    {
        private readonly ThrottleOptions _options;
        private readonly ILogger _logger;
        private readonly ISystemClock _clock;
        private readonly IList<IRequirementHandler> _handlers;
        private readonly IList<IExclusionHandler> _exclusionHandlers;
        private readonly IThrottleCounterStore _store;

        public ThrottleService(
            ILoggerFactory loggerFactory,
            IEnumerable<IRequirementHandler> handlers,
            IEnumerable<IExclusionHandler> exclusionHandlers,
            ISystemClock clock,
            IThrottleCounterStore store,
            IOptions<ThrottleOptions> options,
            ConfigureOptions<ThrottleOptions> configureOptions = null)
        {
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            if (handlers == null)
            {
                throw new ArgumentNullException(nameof(handlers));
            }

            if (exclusionHandlers == null)
            {
                throw new ArgumentNullException(nameof(exclusionHandlers));
            }

            if (clock == null)
            {
                throw new ArgumentNullException(nameof(clock));
            }

            if (store == null)
            {
                throw new ArgumentNullException(nameof(store));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _options = options.Value;
            if (configureOptions != null)
            {
                configureOptions.Configure(_options);
            }

            _options.BuildPolicies();

            _handlers = handlers.ToArray();
            _exclusionHandlers = exclusionHandlers.ToArray();
            _logger = loggerFactory.CreateLogger<ThrottleService>();
            _clock = clock;
            _store = store;
        }

        public virtual async Task<ThrottleContext> EvaluateAsync(HttpContext httpContext, ThrottleStrategy strategy)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            if (strategy == null)
            {
                throw new ArgumentNullException(nameof(strategy));
            }

            var throttleContext = new ThrottleContext(httpContext, strategy, _store);

            foreach (var exclusion in _exclusionHandlers)
            {
                await exclusion.HandleExclusionAsync(throttleContext);
            }

            if (!throttleContext.HasAborted)
            {
                foreach (var handler in _handlers)
                {
                    await handler.HandleRequirementAsync(throttleContext);
                }
            }

            return throttleContext;
        }

        public async Task PostEvaluateAsync(ThrottleContext throttleContext)
        {
            if (throttleContext == null)
            {
                throw new ArgumentNullException(nameof(throttleContext));
            }

            foreach (var handler in _handlers)
            {
                await handler.PostHandleRequirementAsync(throttleContext);
            }
        }
    }
}
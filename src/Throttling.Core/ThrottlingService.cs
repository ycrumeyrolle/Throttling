using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Features;
using Microsoft.Framework.Internal;
using Microsoft.Framework.Logging;
using Microsoft.Framework.OptionsModel;

namespace Throttling
{
    public class ThrottlingService : IThrottlingService
    {
        private readonly ThrottlingOptions _options;
        private readonly ILogger _logger;
        private readonly ISystemClock _clock;
        private readonly IList<IThrottlingHandler> _handlers;

        public ThrottlingService(
            [NotNull] ILoggerFactory loggerFactory, 
            [NotNull] IEnumerable<IThrottlingHandler> handlers,
            [NotNull] ISystemClock clock,
            [NotNull] IOptions<ThrottlingOptions> options, 
            ConfigureOptions<ThrottlingOptions> configureOptions = null)
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
            _logger = loggerFactory.CreateLogger<ThrottlingService>();
            _clock = clock;
        }

        public virtual async Task<ThrottlingContext> EvaluateAsync([NotNull] HttpContext context, [NotNull] ThrottlingStrategy strategy)
        {
            var throttlingContext = new ThrottlingContext(context, strategy);
            if (strategy.Policy.Whitelist != null)
            {
                IHttpConnectionFeature connection = context.GetFeature<IHttpConnectionFeature>();
                if (strategy.Policy.Whitelist.Contains(connection.RemoteIpAddress))
                {
                    return null;
                }
            }

            for (int i = 0; i < _handlers.Count; i++)
            {
                await _handlers[i].HandleAsync(throttlingContext);
            }
           
            return throttlingContext;
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
    }
}
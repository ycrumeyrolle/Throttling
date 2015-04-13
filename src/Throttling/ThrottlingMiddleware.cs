using System;
using System.Threading.Tasks;
using Microsoft.Framework.Internal;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Builder;
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
        private readonly IThrottlingPolicyProvider _throttlingPolicyProvider;
        private readonly ILogger _logger;

        /// <summary>
        /// Instantiates a new <see cref="T:Throttling.ThrottlingMiddleware" />.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        /// <param name="throttlingService">An instance of <see cref="T:Throttling.IThrottlingService" />.</param>
        /// <param name="policy">An instance of the <see cref="T:Throttling.ThrottlingPolicy" /> which can be applied.</param>
        public ThrottlingMiddleware([NotNull] RequestDelegate next, [NotNull] ILoggerFactory loggerFactory, [NotNull] IThrottlingService throttlingService, [NotNull] IThrottlingPolicyProvider policyProvider, [NotNull] IOptions<ThrottlingOptions> options, ConfigureOptions<ThrottlingOptions> configureOptions = null)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<ThrottlingMiddleware>();
            _throttlingService = throttlingService;
            _throttlingPolicyProvider = policyProvider;
        }

        /// <inheritdoc />
        public async Task Invoke(HttpContext context)
        {
            var strategy = await _throttlingPolicyProvider?.GetThrottlingStrategyAsync(context, null);
            if (strategy != null)
            {
                var throttlingResults = await _throttlingService.EvaluateStrategyAsync(context, strategy);
                if (!_throttlingService.ApplyResult(context, throttlingResults))
                {
                    await _next(context);

                    if (context.Response.StatusCode > 99 && context.Response.StatusCode < 300)
                    {
                        await _throttlingService.ApplyLimitAsync(throttlingResults);
                    }
                }
                else
                {
                    await _throttlingService.ApplyLimitAsync(throttlingResults);
                }
            }
            else
            {
                await _next(context);
            }
        }
    }
}

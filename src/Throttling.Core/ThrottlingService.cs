using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;
using Microsoft.Framework.Logging;
using Microsoft.Framework.OptionsModel;

namespace Throttling
{
    public class ThrottlingService : IThrottlingService
    {
        private static readonly Task<IEnumerable<ThrottlingResult>> EmptyResultsTask = Task.FromResult<IEnumerable<ThrottlingResult>>(new ThrottlingResult[0]);

        private readonly ThrottlingOptions _options;
        private readonly ILogger _logger;

        public ThrottlingService([NotNull] ILoggerFactory loggerFactory, [NotNull] IOptions<ThrottlingOptions> options, ConfigureOptions<ThrottlingOptions> configureOptions = null)
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

            _options.ConfigurePolicies();
            _logger = loggerFactory.CreateLogger<ThrottlingService>();
        }

        /// <summary>
        /// Looks up a policy using the <paramref name="policyName"/> and then evaluates the policy using the passed in
        /// <paramref name="context"/>.
        /// </summary>
        /// <param name="requestContext"></param>
        /// <param name="policyName"></param>
        /// <returns>A <see cref="ThrottlingResult"/> which contains the result of policy evaluation.</returns>
        public virtual Task<IEnumerable<ThrottlingResult>> EvaluateStrategyAsync([NotNull] HttpContext context, [NotNull] ThrottlingStrategy strategy)
        {
            strategy.Policy.Configure(_options);
            _logger.LogVerbose(new ThrottlingPolicyLogValues(strategy.Policy));
            if (strategy.Whitelist != null)
            {
                //string xForwadedFor = context.Request.Headers["X-Forwarded-For"];
                //if (xForwadedFor != null)
                //{
                //    var indexOfComma = xForwadedFor.IndexOf(',');
                //}
                IHttpConnectionFeature  connection = context.GetFeature<IHttpConnectionFeature>();
                if (strategy.Whitelist.Contains(connection.RemoteIpAddress))
                {
                    return EmptyResultsTask;
                }
            }

            return strategy.Policy.EvaluateAsync(context, strategy.RouteTemplate);
        }

        /// <inheritsdocs />
        public virtual bool ApplyResult([NotNull] HttpContext context, [NotNull] IEnumerable<ThrottlingResult> results)
        {
            foreach (var header in results.SelectMany(r => r.RateLimitHeaders).OrderBy(h => h.Key))
            {
                context.Response.Headers.Set(header.Key, header.Value);
            }

            if (results.Any(r => r.LimitReached))
            {
                context.Response.StatusCode = Constants.Status429TooManyRequests;

                // rfc6585 section 4 : Responses with the 429 status code MUST NOT be stored by a cache.
                context.Response.Headers.Set("Cache-Control", "no-store");
                context.Response.Headers.Append("Cache-Control", "no-cache");
                context.Response.Headers.Set("Pragma", "no-cache");
                context.Response.Headers.Set("Expires", "-1");

                // rfc6585 section 4 : The response [...] MAY include a Retry-After header indicating how long to wait before making a new request.
                var reset = results.Where(r => r.Reset.HasValue).Max(r => r.Reset);
                if (reset.HasValue)
                {
                    var retryAfterValue = GetRetryAfterValue(_options.RetryAfterMode, reset.Value);
                    if (retryAfterValue != null)
                    {
                        context.Response.Headers.Set("Retry-After", retryAfterValue);
                    }
                }

                _logger.LogVerbose(new ThrottlingResultLogValues(reset));
                return true;
            }

            return false;
        }

        private string GetRetryAfterValue(RetryAfterMode mode, DateTimeOffset reset)
        {
            switch (mode)
            {
                case RetryAfterMode.HttpDate:
                    return reset.ToString("r");
                case RetryAfterMode.DeltaSeconds:
                    return Convert.ToInt64((reset - _options.Clock.UtcNow).TotalSeconds).ToString(CultureInfo.InvariantCulture);
                default:
                    return null;
            }
        }

        public async Task ApplyLimitAsync(IEnumerable<ThrottlingResult> results)
        {
            foreach (var result in results)
            {
                await _options.RateStore.SetRemainingRateAsync(result.Category, result.Endpoint, result.Key, result.Rate);
            }
        }
    }
}
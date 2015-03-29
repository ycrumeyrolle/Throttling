using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;
using Microsoft.Framework.OptionsModel;

namespace Throttling
{
    public class ThrottlingService : IThrottlingService
    {
        private static DateTimeOffset epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private readonly ThrottlingOptions _options;

        public ThrottlingService([NotNull] IOptions<ThrottlingOptions> options, ConfigureOptions<ThrottlingOptions> configureOptions = null)
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
        }


        /// <summary>
        /// Looks up a policy using the <paramref name="policyName"/> and then evaluates the policy using the passed in
        /// <paramref name="context"/>.
        /// </summary>
        /// <param name="requestContext"></param>
        /// <param name="policyName"></param>
        /// <returns>A <see cref="ThrottlingResult"/> which contains the result of policy evaluation.</returns>
        public Task<IEnumerable<ThrottlingResult>> EvaluatePolicyAsync([NotNull] HttpContext context, [NotNull] string policyName)
        {
            var policy = _options.GetPolicy(policyName);
            return policy.EvaluateAsync(context);
        }

        public virtual Task<IEnumerable<ThrottlingResult>> EvaluatePolicyAsync([NotNull] HttpContext context, [NotNull] IThrottlingPolicy policy)
        {
            return policy.EvaluateAsync(context);
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
                context.Response.Headers.Set("Cache-Control", "no-cache");
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

                return true;
            }

            return false;
        }

        public static long ConvertToEpoch(DateTimeOffset dateTime)
        {
            return (dateTime.Ticks - epoch.Ticks) / TimeSpan.TicksPerSecond;
        }

        private string GetRetryAfterValue(RetryAfterMode mode, DateTimeOffset reset)
        {
            switch (mode)
            {
                case RetryAfterMode.HttpDate:
                    return reset.ToString("r");
                case RetryAfterMode.DeltaSeconds:
                    return (reset - _options.Clock.UtcNow).TotalSeconds.ToString(CultureInfo.InvariantCulture);
                default:
                    return null;
            }
        }
    }









    public class RemainingRate
    {
        public DateTimeOffset Reset { get; set; }

        public long Remaining { get; set; }
    }
}
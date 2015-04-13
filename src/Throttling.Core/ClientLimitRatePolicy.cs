using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public class ClientLimitRatePolicy : RateLimitPolicy
    {
        public ClientLimitRatePolicy(long calls, TimeSpan renewalPeriod, bool sliding)
            : base(calls, renewalPeriod, sliding)
        {
            Category = "client";
        }

        public override string GetKey([NotNull] HttpContext context)
        {
            return _options.ClientKeyProvider.GetKey(context);
        }

        public override void AddRateLimitHeaders(RemainingRate rate, IDictionary<string, string> rateLimitHeaders)
        {
            rateLimitHeaders.Add("X-RateLimit-ClientLimit", _calls.ToString(CultureInfo.InvariantCulture));
            rateLimitHeaders.Add("X-RateLimit-ClientRemaining", rate.RemainingCalls.ToString(CultureInfo.InvariantCulture));
        }
    }
}
using System;
using System.Collections.Generic;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Http.Features;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public class IPLimitRatePolicy : RateLimitPolicy
    {
        public IPLimitRatePolicy(long calls, TimeSpan renewalPeriod, bool sliding)
            : base(calls, renewalPeriod, sliding)
        {
            Category = "ip";
        }

        public override void AddRateLimitHeaders(RemainingRate rate, IDictionary<string, string> rateLimitHeaders)
        {
            rateLimitHeaders.Add("IpRate", rate.RemainingCalls.ToString());
        }

        public override string GetKey([NotNull]HttpContext context)
        {
            // TODO : What if behind reverse proxy? X-Forwarded-For ?
            IHttpConnectionFeature connection = context.GetFeature<IHttpConnectionFeature>();

            return connection.RemoteIpAddress.ToString();
        }
    }
}
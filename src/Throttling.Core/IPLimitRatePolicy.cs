using System;
using System.Collections.Generic;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public class IPLimitRatePolicy : LimitRatePolicy
    {
        public IPLimitRatePolicy(long limit, TimeSpan window, bool sliding)
            : base(limit, window, sliding)
        {
            Category = "ip";
        }

        public override void AddRateLimitHeaders(RemainingRate rate, IDictionary<string, string> rateLimitHeaders)
        {
        }

        public override string GetKey([NotNull]HttpContext context)
        {
            // TODO : What if behind reverse proxy? X-Forwarded-For ?
            IHttpConnectionFeature connection = context.GetFeature<IHttpConnectionFeature>();

            return connection.RemoteIpAddress.ToString();
        }
    }
}
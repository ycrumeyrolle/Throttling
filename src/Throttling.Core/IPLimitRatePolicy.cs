using System;
using System.Collections.Generic;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public class IPLimitRatePolicy : LimitRatePolicy
    {
        public IPLimitRatePolicy(ThrottlingOptions options, long limit, TimeSpan window, bool sliding)
            : base(options, limit, window, sliding)
        {
        }

        public override string Category
        {
            get
            {
                return "ip";
            }
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
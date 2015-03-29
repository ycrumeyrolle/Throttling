using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;
using System;
using System.Collections.Generic;

namespace Throttling
{
    public class ClientLimitRatePolicy : LimitRatePolicy
    {
        public override string Category
        {
            get
            {
                return "client";
            }
        }

        public ClientLimitRatePolicy(ThrottlingOptions options, long limit, TimeSpan window, bool sliding)
            : base(options, limit, window, sliding)
        {
        }

        public override string GetKey([NotNull] HttpContext context)
        {
            return "??";
        }

        public override void AddRateLimitHeaders(RemainingRate rate, IDictionary<string, string> rateLimitHeaders)
        {
            rateLimitHeaders.Add("X-RateLimit-ClientLimit", this._limit.ToString());
            rateLimitHeaders.Add("X-RateLimit-ClientRemaining", rate.Remaining.ToString());
        }
    }
}
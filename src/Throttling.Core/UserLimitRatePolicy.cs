using System;
using System.Collections.Generic;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public class UserLimitRatePolicy : LimitRatePolicy
    {
        private readonly IPLimitRatePolicy _fallbackPolicy;

        public override string Category
        {
            get
            {
                return "user";
            }
        }

        public UserLimitRatePolicy(ThrottlingOptions options, long authenticatedLimit, TimeSpan authenticatedWindow, long unauthenticatedLimit, TimeSpan unauthenticatedWindow, bool sliding)
            : base(options, authenticatedLimit, authenticatedWindow, sliding)
        {
            _fallbackPolicy = new IPLimitRatePolicy(options, unauthenticatedLimit, unauthenticatedWindow, sliding);
        }

        public override string GetKey([NotNull]HttpContext context)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                return context.User.Identity.Name;
            }

            return _fallbackPolicy.GetKey(context);
        }

        public override void AddRateLimitHeaders(RemainingRate rate, IDictionary<string, string> rateLimitHeaders)
        {
            rateLimitHeaders.Add("X-RateLimit-UserLimit", _limit.ToString());
            rateLimitHeaders.Add("X-RateLimit-UserRemaining", rate.Remaining.ToString());
            rateLimitHeaders.Add("X-RateLimit-UserReset", ThrottlingService.ConvertToEpoch(rate.Reset).ToString());
        }
    }
}
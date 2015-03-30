using System;
using System.Collections.Generic;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public class UserLimitRatePolicy : LimitRatePolicy
    {
        private readonly IPLimitRatePolicy _fallbackPolicy;

        public UserLimitRatePolicy(long authenticatedLimit, TimeSpan authenticatedWindow, long unauthenticatedLimit, TimeSpan unauthenticatedWindow, bool sliding)
            : base(authenticatedLimit, authenticatedWindow, sliding)
        {
            _fallbackPolicy = new IPLimitRatePolicy(unauthenticatedLimit, unauthenticatedWindow, sliding);
            Category = "user";
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
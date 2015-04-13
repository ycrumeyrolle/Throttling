using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public class UserLimitRatePolicy : RateLimitPolicy
    {
        private readonly IPLimitRatePolicy _fallbackPolicy;

        public UserLimitRatePolicy(long authenticatedCalls, TimeSpan authenticatedRenewalPeriod, long unauthenticatedCalls, TimeSpan unauthenticatedRenewalPeriod, bool sliding)
            : base(authenticatedCalls, authenticatedRenewalPeriod, sliding)
        {
            Category = "user";
            _fallbackPolicy = new IPLimitRatePolicy(unauthenticatedCalls, unauthenticatedRenewalPeriod, sliding);
            _fallbackPolicy.Category = Category + "_" + _fallbackPolicy.Category;
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
            rateLimitHeaders.Add("X-RateLimit-UserLimit", _calls.ToString(CultureInfo.InvariantCulture));
            rateLimitHeaders.Add("X-RateLimit-UserRemaining", rate.RemainingCalls.ToString(CultureInfo.InvariantCulture));
            rateLimitHeaders.Add("X-RateLimit-UserReset", rate.Reset.ToEpoch().ToString(CultureInfo.InvariantCulture));
        }
    }
}
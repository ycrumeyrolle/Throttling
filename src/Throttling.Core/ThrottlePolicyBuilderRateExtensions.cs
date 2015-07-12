using System;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public static class ThrottlePolicyBuilderRateExtensions
    {
        /// <summary>
        /// Limits the calls for an authenticated user.
        /// </summary>
        /// <param name="calls">The number of possible calls per period after which access is denied.</param>
        /// <param name="renewalPeriod">The period of time until calls count is renewed.</param>
        /// <param name="sliding">Indicates wether the renewal period is sliding.</param>
        /// <returns>The current policy builder.</returns>
        public static ThrottlePolicyBuilder LimitAuthenticatedUserRate([NotNull] this ThrottlePolicyBuilder builder, long calls, TimeSpan renewalPeriod, bool sliding = false)
        {
            return builder.AddRequirements(new AuthenticatedUserRateLimitRequirement(calls, renewalPeriod, sliding));
        }

        /// <summary>
        /// Limits the calls by IP address.
        /// </summary>
        /// <param name="calls">The number of possible calls per period after which access is denied.</param>
        /// <param name="renewalPeriod">The period of time until calls count is renewed.</param>
        /// <param name="sliding">Indicates wether the renewal period is sliding.</param>
        /// <returns>The current policy builder.</returns>
        public static ThrottlePolicyBuilder LimitIPRate([NotNull] this ThrottlePolicyBuilder builder, long calls, TimeSpan renewalPeriod, bool sliding = false)
        {
            return builder.AddRequirements(new IPRateLimitRequirement(calls, renewalPeriod, sliding));
        }

        /// <summary>
        /// Limits the calls by a specified <paramref name="formParameter"/>.
        /// </summary>
        /// <param name="formParameter">The name of the parameter to look at.</param>
        /// <param name="calls">The number of possible calls per period after which access is denied.</param>
        /// <param name="renewalPeriod">The period of time until calls count is renewed.</param>
        /// <param name="sliding">Indicates wether the renewal period is sliding.</param>
        /// <returns>The current policy builder.</returns>
        public static ThrottlePolicyBuilder LimitClientRateByFormParameter([NotNull] this ThrottlePolicyBuilder builder, string formParameter, long calls, TimeSpan renewalPeriod, bool sliding = false)
        {
            return builder.AddRequirements(new FormApiKeyRateLimitRequirement(calls, renewalPeriod, sliding, formParameter));
        }

        /// <summary>
        /// Limits the calls by a specified <paramref name="headerName"/>.
        /// </summary>
        /// <param name="headerName">The name of the HTTP header to look at.</param>
        /// <param name="calls">The number of possible calls per period after which access is denied.</param>
        /// <param name="renewalPeriod">The period of time until calls count is renewed.</param>
        /// <param name="sliding">Indicates wether the renewal period is sliding.</param>
        /// <returns>The current policy builder.</returns>
        public static ThrottlePolicyBuilder LimitClientRateByHeader([NotNull] this ThrottlePolicyBuilder builder, string headerName, long calls, TimeSpan renewalPeriod, bool sliding = false)
        {
            return builder.AddRequirements(new HeaderApiKeyRateLimitRequirement(calls, renewalPeriod, sliding, headerName));
        }

        /// <summary>
        /// Limits the calls by a specified <paramref name="queryStringParameter"/>.
        /// </summary>
        /// <param name="headerName">The name of the query string parameter to look at.</param>
        /// <param name="calls">The number of possible calls per period after which access is denied.</param>
        /// <param name="renewalPeriod">The period of time until calls count is renewed.</param>
        /// <param name="sliding">Indicates wether the renewal period is sliding.</param>
        /// <returns>The current policy builder.</returns>
        public static ThrottlePolicyBuilder LimitClientRateByQueryStringParameter([NotNull] this ThrottlePolicyBuilder builder, string queryStringParameter, long calls, TimeSpan renewalPeriod, bool sliding = false)
        {
            return builder.AddRequirements(new QueryStringApiKeyRateLimitRequirement(calls, renewalPeriod, sliding, queryStringParameter));
        }

        /// <summary>
        /// Limits the calls by a specified <paramref name="routeTemplate"/>.
        /// </summary>
        /// <param name="routeTemplate">The route template. For example {apikey}/{*remaining} where "apikey" is the <paramref name="routeFragment"/>.</param>
        /// <param name="routeFragment">The name of the route fragment to look at in the route template.</param>
        /// <param name="calls">The number of possible calls per period after which access is denied.</param>
        /// <param name="renewalPeriod">The period of time until calls count is renewed.</param>
        /// <param name="sliding">Indicates wether the renewal period is sliding.</param>
        /// <returns>The current policy builder.</returns>
        public static ThrottlePolicyBuilder LimitClientRateByRoute([NotNull] this ThrottlePolicyBuilder builder, string routeTemplate, string routeFragment, long calls, TimeSpan renewalPeriod, bool sliding = false)
        {
            return builder.AddRequirements(new RouteApiKeyRateLimitRequirement(calls, renewalPeriod, sliding, routeTemplate, routeFragment));
        }
    }
}
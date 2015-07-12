using System;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public static class ThrottlePolicyBuilderBandwidthExtensions
    {
        /// <summary>
        /// Limits the bandwidth for an authenticated user.
        /// </summary>
        /// <param name="bandwidth">The maximum bandwidth per period after which access is denied.</param>
        /// <param name="renewalPeriod">The period of time until bandwidth count is renewed.</param>
        /// <param name="sliding">Indicates wether the renewal period is sliding.</param>
        /// <returns>The current policy builder.</returns>
        public static ThrottlePolicyBuilder LimitAuthenticatedUserBandwidth([NotNull] this ThrottlePolicyBuilder builder, long bandwidth, TimeSpan renewalPeriod, bool sliding = false)
        {
            return builder.AddRequirements(new AuthenticatedUserBandwidthRequirement(bandwidth, renewalPeriod, sliding));
        }

        /// <summary>
        /// Limits the bandwidth by IP address.
        /// </summary>
        /// <param name="bandwidth">The maximum bandwidth per period after which access is denied.</param>
        /// <param name="renewalPeriod">The period of time until bandwidth count is renewed.</param>
        /// <param name="sliding">Indicates wether the renewal period is sliding.</param>
        /// <returns>The current policy builder.</returns>
        public static ThrottlePolicyBuilder LimitIPBandwidth([NotNull] this ThrottlePolicyBuilder builder, long bandwidth, TimeSpan renewalPeriod, bool sliding = false)
        {
            return builder.AddRequirements(new IPBandwidthRequirement(bandwidth, renewalPeriod, sliding));
        }

        /// <summary>
        /// Limits the bandwidth by a specified <paramref name="formParameter"/>.
        /// </summary>
        /// <param name="formParameter">The name of the parameter to look at.</param>
        /// <param name="bandwidth">The maximum bandwidth per period after which access is denied.</param>
        /// <param name="renewalPeriod">The period of time until bandwidth count is renewed.</param>
        /// <param name="sliding">Indicates wether the renewal period is sliding.</param>
        /// <returns>The current policy builder.</returns>
        public static ThrottlePolicyBuilder LimitClientBandwidthByFormParameter([NotNull] this ThrottlePolicyBuilder builder, string formParameter, long bandwidth, TimeSpan renewalPeriod, bool sliding = false)
        {
            return builder.AddRequirements(new FormApiKeyBandwidthRequirement(bandwidth, renewalPeriod, sliding, formParameter));
        }

        /// <summary>
        /// Limits the bandwidth by a specified <paramref name="headerName"/>.
        /// </summary>
        /// <param name="headerName">The name of the HTTP header to look at.</param>
        /// <param name="bandwidth">The maximum bandwidth per period after which access is denied.</param>
        /// <param name="renewalPeriod">The period of time until bandwidth count is renewed.</param>
        /// <param name="sliding">Indicates wether the renewal period is sliding.</param>
        /// <returns>The current policy builder.</returns>
        public static ThrottlePolicyBuilder LimitClienBandwidthByHeader([NotNull] this ThrottlePolicyBuilder builder, string headerName, long bandwidth, TimeSpan renewalPeriod, bool sliding = false)
        {
            return builder.AddRequirements(new HeaderApiKeyRateLimitRequirement(bandwidth, renewalPeriod, sliding, headerName));
        }

        /// <summary>
        /// Limits the bandwidth by a specified <paramref name="queryStringParameter"/>.
        /// </summary>
        /// <param name="headerName">The name of the query string parameter to look at.</param>
        /// <param name="bandwidth">The maximum bandwidth per period after which access is denied.</param>
        /// <param name="renewalPeriod">The period of time until bandwidth count is renewed.</param>
        /// <param name="sliding">Indicates wether the renewal period is sliding.</param>
        /// <returns>The current policy builder.</returns>
        public static ThrottlePolicyBuilder LimitClientBandwidthByQueryStringParameter([NotNull] this ThrottlePolicyBuilder builder, string queryStringParameter, long bandwidth, TimeSpan renewalPeriod, bool sliding = false)
        {
            return builder.AddRequirements(new QueryStringApiKeyRateLimitRequirement(bandwidth, renewalPeriod, sliding, queryStringParameter));
        }

        /// <summary>
        /// Limits the bandwidth by a specified <paramref name="routeTemplate"/>.
        /// </summary>
        /// <param name="routeTemplate">The route template. For example {apikey}/{*remaining} where "apikey" is the <paramref name="routeFragment"/>.</param>
        /// <param name="routeFragment">The name of the route fragment to look at in the route template.</param>
        /// <param name="bandwidth">The maximum bandwidth per period after which access is denied.</param>
        /// <param name="renewalPeriod">The period of time until bandwidth count is renewed.</param>
        /// <param name="sliding">Indicates wether the renewal period is sliding.</param>
        /// <returns>The current policy builder.</returns>
        public static ThrottlePolicyBuilder LimitClientBandwidthByRoute([NotNull] this ThrottlePolicyBuilder builder, string routeTemplate, string routeFragment, long bandwidth, TimeSpan renewalPeriod, bool sliding = false)
        {
            return builder.AddRequirements(new RouteApiKeyBandwidthRequirement(bandwidth, renewalPeriod, sliding, routeTemplate, routeFragment));
        }
    }
}
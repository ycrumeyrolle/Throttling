using System;
using System.Collections.Generic;
using Microsoft.Framework.Internal;
using Throttling.IPRanges;

namespace Throttling
{
    public class ThrottlingPolicyBuilder
    {
        private readonly string _policyName;

        public ThrottlingPolicyBuilder(string policyName)
        {
            _policyName = policyName;
        }

        public IList<IThrottlingRequirement> Requirements { get; set; } = new List<IThrottlingRequirement>();
        
        private IList<IPAddressRange> Whitelist { get; set; } = new List<IPAddressRange>();

        /// <summary>
        /// Adds the specified <paramref name="headers"/> to the policy.
        /// </summary>
        /// <param name="headers">The headers which need to be allowed in the request.</param>
        /// <returns>The current policy builder</returns>
        public ThrottlingPolicyBuilder LimitAuthenticatedUserRate(long calls, TimeSpan renewalPeriod, bool sliding = false)
        {
            return AddRequirements(new AuthenticatedUserRateLimitRequirement(calls, renewalPeriod, sliding));

        }
        
        /// <summary>
        /// Adds the specified <paramref name="headers"/> to the policy.
        /// </summary>
        /// <param name="headers">The headers which need to be allowed in the request.</param>
        /// <returns>The current policy builder</returns>
        public ThrottlingPolicyBuilder LimitIPRate(long calls, TimeSpan renewalPeriod, bool sliding = false)
        {
            return AddRequirements(new IPRateLimitRequirement(calls, renewalPeriod, sliding));
        }
        
        /// <summary>
        /// Adds the specified <paramref name="headers"/> to the policy.
        /// </summary>
        /// <param name="headers">The headers which need to be allowed in the request.</param>
        /// <returns>The current policy builder</returns>
        public ThrottlingPolicyBuilder LimitClientRateByFormParameter(string formParamater, long calls, TimeSpan renewalPeriod, bool sliding = false)
        {
            return AddRequirements(new FormApiKeyRateLimitRequirement(calls, renewalPeriod, sliding, formParamater));
        }

        /// <summary>
        /// Adds the specified <paramref name="headers"/> to the policy.
        /// </summary>
        /// <param name="headers">The headers which need to be allowed in the request.</param>
        /// <returns>The current policy builder</returns>
        public ThrottlingPolicyBuilder LimitClientRateByHeader(string headerName, long calls, TimeSpan renewalPeriod, bool sliding = false)
        {
            return AddRequirements(new HeaderApiKeyRateLimitRequirement(calls, renewalPeriod, sliding, headerName));
        }

        /// <summary>
        /// Adds the specified <paramref name="headers"/> to the policy.
        /// </summary>
        /// <param name="headers">The headers which need to be allowed in the request.</param>
        /// <returns>The current policy builder</returns>
        public ThrottlingPolicyBuilder LimitClientRateByQueryStringParameter(string queryStringParameter, long calls, TimeSpan renewalPeriod, bool sliding = false)
        {
            return AddRequirements(new QueryStringApiKeyRateLimitRequirement(calls, renewalPeriod, sliding, queryStringParameter));
        }

        /// <summary>
        /// Adds the specified <paramref name="headers"/> to the policy.
        /// </summary>
        /// <param name="headers">The headers which need to be allowed in the request.</param>
        /// <returns>The current policy builder</returns>
        public ThrottlingPolicyBuilder LimitClientRateByRoute(string routeTemplate, string routeFragment, long calls, TimeSpan renewalPeriod, bool sliding = false)
        {
            return AddRequirements(new RouteApiKeyRateLimitRequirement(calls, renewalPeriod, sliding, routeTemplate , routeFragment));
        }

        /// <summary>
        /// Adds the specified <paramref name="headers"/> to the policy.
        /// </summary>
        /// <param name="headers">The headers which need to be allowed in the request.</param>
        /// <returns>The current policy builder</returns>
        public ThrottlingPolicyBuilder LimitAuthenticatedUserBandwidth(long bandwidth, TimeSpan renewalPeriod, bool sliding = false)
        {
            return AddRequirements(new AuthenticatedUserBandwidthRequirement(bandwidth, renewalPeriod, sliding));
        }

        /// <summary>
        /// Adds the specified <paramref name="headers"/> to the policy.
        /// </summary>
        /// <param name="headers">The headers which need to be allowed in the request.</param>
        /// <returns>The current policy builder</returns>
        public ThrottlingPolicyBuilder LimitIPBandwidth(long bandwidth, TimeSpan renewalPeriod, bool sliding = false)
        {
            return AddRequirements(new IPBandwidthRequirement(bandwidth, renewalPeriod, sliding));
        }

        /// <summary>
        /// Adds the specified <paramref name="headers"/> to the policy.
        /// </summary>
        /// <param name="headers">The headers which need to be allowed in the request.</param>
        /// <returns>The current policy builder</returns>
        public ThrottlingPolicyBuilder LimitClientBandwidthByFormParameter(string formParamater, long bandwidth, TimeSpan renewalPeriod, bool sliding = false)
        {
            return AddRequirements(new FormApiKeyBandwidthRequirement(bandwidth, renewalPeriod, sliding, formParamater));
        }

        /// <summary>
        /// Adds the specified <paramref name="headers"/> to the policy.
        /// </summary>
        /// <param name="headers">The headers which need to be allowed in the request.</param>
        /// <returns>The current policy builder</returns>
        public ThrottlingPolicyBuilder LimitClienBandwidthByHeader(string headerName, long bandwidth, TimeSpan renewalPeriod, bool sliding = false)
        {
            return AddRequirements(new HeaderApiKeyRateLimitRequirement(bandwidth, renewalPeriod, sliding, headerName));
        }

        /// <summary>
        /// Adds the specified <paramref name="headers"/> to the policy.
        /// </summary>
        /// <param name="headers">The headers which need to be allowed in the request.</param>
        /// <returns>The current policy builder</returns>
        public ThrottlingPolicyBuilder LimitClientBandwidthByQueryStringParameter(string queryStringParameter, long bandwidth, TimeSpan renewalPeriod, bool sliding = false)
        {
            return AddRequirements(new QueryStringApiKeyRateLimitRequirement(bandwidth, renewalPeriod, sliding, queryStringParameter));
        }

        /// <summary>
        /// Adds the specified <paramref name="headers"/> to the policy.
        /// </summary>
        /// <param name="headers">The headers which need to be allowed in the request.</param>
        /// <returns>The current policy builder</returns>
        public ThrottlingPolicyBuilder LimitClientBandwidthByRoute(string routeTemplate, string routeFragment, long bandwidth, TimeSpan renewalPeriod, bool sliding = false)
        {
            return AddRequirements(new RouteApiKeyRateLimitRequirement(bandwidth, renewalPeriod, sliding, routeTemplate, routeFragment));
        }

        /// <summary>
        /// Adds the specified <paramref name="headers"/> to the policy.
        /// </summary>
        /// <param name="headers">The headers which need to be allowed in the request.</param>
        /// <returns>The current policy builder</returns>
        public ThrottlingPolicyBuilder AddRequirements(params IThrottlingRequirement[] requirements)
        {
            foreach (var requirement in requirements)
            {
                Requirements.Add(requirement);
            }

            return this;
        }

        /// <summary>
        /// Adds the IP range to the IP whitelist. 
        /// These addresses will pass through the policy.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public ThrottlingPolicyBuilder IgnoreIPAddressRange([NotNull] string range)
        {            
            var addressRange = IPAddressRange.Parse(range);
            Whitelist.Add(addressRange);
            return this;
        }

        public ThrottlingPolicy Build()
        {
            return new ThrottlingPolicy(Requirements, Whitelist, _policyName);
        }
    }
}
using System;
using System.Collections.Generic;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public class ThrottlePolicyBuilder
    {
        private readonly string _policyName;

        public ThrottlePolicyBuilder(string policyName)
        {
            _policyName = policyName;
        }
        
        public IList<IThrottleRequirement> Requirements { get; } = new List<IThrottleRequirement>();
        
        public IList<IThrottleExclusion> Exclusions { get; } = new List<IThrottleExclusion>();

        /// <summary>
        /// Adds the specified <paramref name="headers"/> to the policy.
        /// </summary>
        /// <param name="headers">The headers which need to be allowed in the request.</param>
        /// <returns>The current policy builder</returns>
        public ThrottlePolicyBuilder LimitAuthenticatedUserRate(long calls, TimeSpan renewalPeriod, bool sliding = false)
        {
            return AddRequirements(new AuthenticatedUserRateLimitRequirement(calls, renewalPeriod, sliding));
        }
        
        /// <summary>
        /// Adds the specified <paramref name="headers"/> to the policy.
        /// </summary>
        /// <param name="headers">The headers which need to be allowed in the request.</param>
        /// <returns>The current policy builder</returns>
        public ThrottlePolicyBuilder LimitIPRate(long calls, TimeSpan renewalPeriod, bool sliding = false)
        {
            return AddRequirements(new IPRateLimitRequirement(calls, renewalPeriod, sliding));
        }
        
        /// <summary>
        /// Adds the specified <paramref name="headers"/> to the policy.
        /// </summary>
        /// <param name="headers">The headers which need to be allowed in the request.</param>
        /// <returns>The current policy builder</returns>
        public ThrottlePolicyBuilder LimitClientRateByFormParameter(string formParamater, long calls, TimeSpan renewalPeriod, bool sliding = false)
        {
            return AddRequirements(new FormApiKeyRateLimitRequirement(calls, renewalPeriod, sliding, formParamater));
        }

        /// <summary>
        /// Adds the specified <paramref name="headers"/> to the policy.
        /// </summary>
        /// <param name="headers">The headers which need to be allowed in the request.</param>
        /// <returns>The current policy builder</returns>
        public ThrottlePolicyBuilder LimitClientRateByHeader(string headerName, long calls, TimeSpan renewalPeriod, bool sliding = false)
        {
            return AddRequirements(new HeaderApiKeyRateLimitRequirement(calls, renewalPeriod, sliding, headerName));
        }

        /// <summary>
        /// Adds the specified <paramref name="headers"/> to the policy.
        /// </summary>
        /// <param name="headers">The headers which need to be allowed in the request.</param>
        /// <returns>The current policy builder</returns>
        public ThrottlePolicyBuilder LimitClientRateByQueryStringParameter(string queryStringParameter, long calls, TimeSpan renewalPeriod, bool sliding = false)
        {
            return AddRequirements(new QueryStringApiKeyRateLimitRequirement(calls, renewalPeriod, sliding, queryStringParameter));
        }

        /// <summary>
        /// Adds the specified <paramref name="headers"/> to the policy.
        /// </summary>
        /// <param name="headers">The headers which need to be allowed in the request.</param>
        /// <returns>The current policy builder</returns>
        public ThrottlePolicyBuilder LimitClientRateByRoute(string routeTemplate, string routeFragment, long calls, TimeSpan renewalPeriod, bool sliding = false)
        {
            return AddRequirements(new RouteApiKeyRateLimitRequirement(calls, renewalPeriod, sliding, routeTemplate , routeFragment));
        }

        /// <summary>
        /// Adds the specified <paramref name="headers"/> to the policy.
        /// </summary>
        /// <param name="headers">The headers which need to be allowed in the request.</param>
        /// <returns>The current policy builder</returns>
        public ThrottlePolicyBuilder LimitAuthenticatedUserBandwidth(long bandwidth, TimeSpan renewalPeriod, bool sliding = false)
        {
            return AddRequirements(new AuthenticatedUserBandwidthRequirement(bandwidth, renewalPeriod, sliding));
        }

        /// <summary>
        /// Adds the specified <paramref name="headers"/> to the policy.
        /// </summary>
        /// <param name="headers">The headers which need to be allowed in the request.</param>
        /// <returns>The current policy builder</returns>
        public ThrottlePolicyBuilder LimitIPBandwidth(long bandwidth, TimeSpan renewalPeriod, bool sliding = false)
        {
            return AddRequirements(new IPBandwidthRequirement(bandwidth, renewalPeriod, sliding));
        }

        /// <summary>
        /// Adds the specified <paramref name="headers"/> to the policy.
        /// </summary>
        /// <param name="headers">The headers which need to be allowed in the request.</param>
        /// <returns>The current policy builder</returns>
        public ThrottlePolicyBuilder LimitClientBandwidthByFormParameter(string formParamater, long bandwidth, TimeSpan renewalPeriod, bool sliding = false)
        {
            return AddRequirements(new FormApiKeyBandwidthRequirement(bandwidth, renewalPeriod, sliding, formParamater));
        }

        /// <summary>
        /// Adds the specified <paramref name="headers"/> to the policy.
        /// </summary>
        /// <param name="headers">The headers which need to be allowed in the request.</param>
        /// <returns>The current policy builder</returns>
        public ThrottlePolicyBuilder LimitClienBandwidthByHeader(string headerName, long bandwidth, TimeSpan renewalPeriod, bool sliding = false)
        {
            return AddRequirements(new HeaderApiKeyRateLimitRequirement(bandwidth, renewalPeriod, sliding, headerName));
        }

        /// <summary>
        /// Adds the specified <paramref name="headers"/> to the policy.
        /// </summary>
        /// <param name="headers">The headers which need to be allowed in the request.</param>
        /// <returns>The current policy builder</returns>
        public ThrottlePolicyBuilder LimitClientBandwidthByQueryStringParameter(string queryStringParameter, long bandwidth, TimeSpan renewalPeriod, bool sliding = false)
        {
            return AddRequirements(new QueryStringApiKeyRateLimitRequirement(bandwidth, renewalPeriod, sliding, queryStringParameter));
        }

        /// <summary>
        /// Adds the specified <paramref name="headers"/> to the policy.
        /// </summary>
        /// <param name="headers">The headers which need to be allowed in the request.</param>
        /// <returns>The current policy builder</returns>
        public ThrottlePolicyBuilder LimitClientBandwidthByRoute(string routeTemplate, string routeFragment, long bandwidth, TimeSpan renewalPeriod, bool sliding = false)
        {
            return AddRequirements(new RouteApiKeyBandwidthRequirement(bandwidth, renewalPeriod, sliding, routeTemplate, routeFragment));
        }

        /// <summary>
        /// Adds the specified <paramref name="headers"/> to the policy.
        /// </summary>
        /// <param name="headers">The headers which need to be allowed in the request.</param>
        /// <returns>The current policy builder</returns>
        public ThrottlePolicyBuilder AddRequirements(params IThrottleRequirement[] requirements)
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
        public ThrottlePolicyBuilder IgnoreIPAddressRanges([NotNull] params string[] ranges)
        {            
            Exclusions.Add(new IPExclusion(ranges));
            return this;
        }

        public ThrottlePolicy Build()
        {
            return new ThrottlePolicy(Requirements, Exclusions, _policyName);
        }
    }
}
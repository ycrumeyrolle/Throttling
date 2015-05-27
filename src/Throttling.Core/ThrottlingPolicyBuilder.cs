using System;
using System.Collections.Generic;
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
        
        private IList<IPAddressRange> WhiteList { get; set; } = new List<IPAddressRange>();

        /// <summary>
        /// Adds the specified <paramref name="headers"/> to the policy.
        /// </summary>
        /// <param name="headers">The headers which need to be allowed in the request.</param>
        /// <returns>The current policy builder</returns>
        public ThrottlingPolicyBuilder LimitAuthenticatedUserRate(long calls, TimeSpan renewalPeriod, bool sliding = false)
        {
            return LimitUserRate(calls, renewalPeriod, 0, renewalPeriod, sliding);
        }

        /// <summary>
        /// Adds the specified <paramref name="headers"/> to the policy.
        /// </summary>
        /// <param name="headers">The headers which need to be allowed in the request.</param>
        /// <returns>The current policy builder</returns>
        public ThrottlingPolicyBuilder LimitUserRate(long authenticatedCalls, TimeSpan authenticatedRenewalPeriod, long unauthenticatedCalls, TimeSpan unauthenticatedRenewalPeriod, bool sliding = false)
        {
            Requirements.Add(new UserLimitRateRequirement(authenticatedCalls, authenticatedRenewalPeriod, sliding, new IPLimitRateRequirement(unauthenticatedCalls, unauthenticatedRenewalPeriod, sliding)));
            return this;
        }

        /// <summary>
        /// Adds the specified <paramref name="headers"/> to the policy.
        /// </summary>
        /// <param name="headers">The headers which need to be allowed in the request.</param>
        /// <returns>The current policy builder</returns>
        public ThrottlingPolicyBuilder LimitIPRate(long calls, TimeSpan renewalPeriod, bool sliding = false)
        {
            Requirements.Add(new IPLimitRateRequirement(calls, renewalPeriod, sliding));
            return this;
        }
        
        /// <summary>
        /// Adds the specified <paramref name="headers"/> to the policy.
        /// </summary>
        /// <param name="headers">The headers which need to be allowed in the request.</param>
        /// <returns>The current policy builder</returns>
        public ThrottlingPolicyBuilder LimitClientRateByFormParameter(string formParamater, long calls, TimeSpan renewalPeriod, bool sliding = false)
        {
            Requirements.Add(new FormApiKeyLimitRateRequirement(calls, renewalPeriod, sliding));
            return this;
        }

        /// <summary>
        /// Adds the specified <paramref name="headers"/> to the policy.
        /// </summary>
        /// <param name="headers">The headers which need to be allowed in the request.</param>
        /// <returns>The current policy builder</returns>
        public ThrottlingPolicyBuilder LimitClientRateByHeader(string headerName, long calls, TimeSpan renewalPeriod, bool sliding = false)
        {
            Requirements.Add(new HeaderApiKeyLimitRateRequirement(calls, renewalPeriod, sliding));
            return this;
        }

        /// <summary>
        /// Adds the specified <paramref name="headers"/> to the policy.
        /// </summary>
        /// <param name="headers">The headers which need to be allowed in the request.</param>
        /// <returns>The current policy builder</returns>
        public ThrottlingPolicyBuilder LimitClientRateByQueryStringParameter(string queryStringParameter, long calls, TimeSpan renewalPeriod, bool sliding = false)
        {
            Requirements.Add(new QueryStringApiKeyLimitRateRequirement(calls, renewalPeriod, sliding));
            return this;
        }

        /// <summary>
        /// Adds the specified <paramref name="headers"/> to the policy.
        /// </summary>
        /// <param name="headers">The headers which need to be allowed in the request.</param>
        /// <returns>The current policy builder</returns>
        public ThrottlingPolicyBuilder LimitClientRateByRoute(string routeTemplate, string routeFragment,long calls, TimeSpan renewalPeriod, bool sliding = false)
        {
            Requirements.Add(new RouteApiKeyLimitRateRequirement(calls, renewalPeriod, sliding));
            return this;
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
        public ThrottlingPolicyBuilder IgnoreIPAddressRange(string range)
        {            
            var addressRange = IPAddressRange.Parse(range);
            WhiteList.Add(addressRange);
            return this;
        }

        public ThrottlingPolicy Build()
        {
            return new ThrottlingPolicy(Requirements, WhiteList, _policyName);
        }
    }
}
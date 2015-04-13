using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public class ThrottlingPolicyBuilder
    {
        private readonly ThrottlingPolicy _policy = new ThrottlingPolicy();
        private readonly string _policyName;
     
        public ThrottlingPolicyBuilder(string policyName)
        {
            _policyName = policyName;
        }

        /// <summary>
        /// Adds the specified <paramref name="headers"/> to the policy.
        /// </summary>
        /// <param name="headers">The headers which need to be allowed in the request.</param>
        /// <returns>The current policy builder</returns>
        public ThrottlingPolicyBuilder AddUserLimitRate(long authenticatedLimit, TimeSpan authenticatedRenewalPeriod, bool sliding = false)
        {
            return AddUserLimitRate(authenticatedLimit, authenticatedRenewalPeriod, 0, authenticatedRenewalPeriod, sliding);
        }

        /// <summary>
        /// Adds the specified <paramref name="headers"/> to the policy.
        /// </summary>
        /// <param name="headers">The headers which need to be allowed in the request.</param>
        /// <returns>The current policy builder</returns>
        public ThrottlingPolicyBuilder AddUserLimitRate(long authenticatedLimit, TimeSpan authenticatedRenewalPeriod, long unauthenticatedLimit, TimeSpan unauthenticatedRenewalPeriod, bool sliding = false)
        {
            return AddPolicy(new UserLimitRatePolicy(authenticatedLimit, authenticatedRenewalPeriod, unauthenticatedLimit, unauthenticatedRenewalPeriod, sliding));
        }

        /// <summary>
        /// Adds the specified <paramref name="headers"/> to the policy.
        /// </summary>
        /// <param name="headers">The headers which need to be allowed in the request.</param>
        /// <returns>The current policy builder</returns>
        public ThrottlingPolicyBuilder AddUserLimitRatePerHour(long calls, bool sliding = false)
        {
            return AddUserLimitRate(calls, TimeSpan.FromHours(1), sliding);
        }

        /// <summary>
        /// Adds the specified <paramref name="headers"/> to the policy.
        /// </summary>
        /// <param name="headers">The headers which need to be allowed in the request.</param>
        /// <returns>The current policy builder</returns>
        public ThrottlingPolicyBuilder AddUserLimitRatePerDay(long calls)
        {
            return AddUserLimitRate(calls, TimeSpan.FromDays(1), false);
        }

        /// <summary>
        /// Adds the specified <paramref name="headers"/> to the policy.
        /// </summary>
        /// <param name="headers">The headers which need to be allowed in the request.</param>
        /// <returns>The current policy builder</returns>
        public ThrottlingPolicyBuilder AddIPLimitRate(long calls, TimeSpan renewalPeriod, bool sliding = false)
        {
            return AddPolicy(new IPLimitRatePolicy(calls, renewalPeriod, sliding));
        }

        /// <summary>
        /// Adds the specified <paramref name="headers"/> to the policy.
        /// </summary>
        /// <param name="headers">The headers which need to be allowed in the request.</param>
        /// <returns>The current policy builder</returns>
        public ThrottlingPolicyBuilder AddIPLimitRatePerHour(long calls)
        {
            return AddIPLimitRate(calls, TimeSpan.FromHours(1), false);
        }

        /// <summary>
        /// Adds the specified <paramref name="headers"/> to the policy.
        /// </summary>
        /// <param name="headers">The headers which need to be allowed in the request.</param>
        /// <returns>The current policy builder</returns>
        public ThrottlingPolicyBuilder AddIPLimitRatePerDay(long calls)
        {
            return AddIPLimitRate(calls, TimeSpan.FromDays(1), false);
        }

        /// <summary>
        /// Adds the specified <paramref name="headers"/> to the policy.
        /// </summary>
        /// <param name="headers">The headers which need to be allowed in the request.</param>
        /// <returns>The current policy builder</returns>
        public ThrottlingPolicyBuilder AddClientLimitRate(long calls, TimeSpan renewalPeriod, bool sliding = false)
        {
            return AddPolicy(new ClientLimitRatePolicy(calls, renewalPeriod, sliding));
        }

        /// <summary>
        /// Adds the specified <paramref name="headers"/> to the policy.
        /// </summary>
        /// <param name="headers">The headers which need to be allowed in the request.</param>
        /// <returns>The current policy builder</returns>
        public ThrottlingPolicyBuilder AddClientLimitRatePerHour(long calls, TimeSpan renewalPeriod)
        {
            return AddIPLimitRate(calls, TimeSpan.FromHours(1), false);
        }

        /// <summary>
        /// Adds the specified <paramref name="headers"/> to the policy.
        /// </summary>
        /// <param name="headers">The headers which need to be allowed in the request.</param>
        /// <returns>The current policy builder</returns>
        public ThrottlingPolicyBuilder AddClientLimitRatePerDay(long calls, TimeSpan renewalPeriod)
        {
            return AddIPLimitRate(calls, TimeSpan.FromDays(1), false);
        }

        /// <summary>
        /// Adds the specified <paramref name="headers"/> to the policy.
        /// </summary>
        /// <param name="headers">The headers which need to be allowed in the request.</param>
        /// <returns>The current policy builder</returns>
        public ThrottlingPolicyBuilder AddPolicy(IThrottlingPolicy policy)
        {
            _policy.AddPolicy(policy);

            return this;
        }

        public IThrottlingPolicy Build()
        {
            _policy.Name = _policyName;
            return _policy;
        }
    }
}
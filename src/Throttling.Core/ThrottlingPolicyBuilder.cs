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
        
        /// <summary>
        /// Adds the specified <paramref name="headers"/> to the policy.
        /// </summary>
        /// <param name="headers">The headers which need to be allowed in the request.</param>
        /// <returns>The current policy builder</returns>
        public ThrottlingPolicyBuilder AddUserLimitRate(long authenticatedLimit, TimeSpan authenticatedWindow, bool sliding = false)
        {
            return AddUserLimitRate(authenticatedLimit, authenticatedWindow, 0, authenticatedWindow, sliding);
        }

        /// <summary>
        /// Adds the specified <paramref name="headers"/> to the policy.
        /// </summary>
        /// <param name="headers">The headers which need to be allowed in the request.</param>
        /// <returns>The current policy builder</returns>
        public ThrottlingPolicyBuilder AddUserLimitRate(long authenticatedLimit, TimeSpan authenticatedWindow, long unauthenticatedLimit, TimeSpan unauthenticatedWindow, bool sliding = false)
        {
            return AddPolicy(new UserLimitRatePolicy(authenticatedLimit, authenticatedWindow, unauthenticatedLimit, unauthenticatedWindow, sliding));
        }

        /// <summary>
        /// Adds the specified <paramref name="headers"/> to the policy.
        /// </summary>
        /// <param name="headers">The headers which need to be allowed in the request.</param>
        /// <returns>The current policy builder</returns>
        public ThrottlingPolicyBuilder AddUserLimitRatePerHour(long limit)
        {
            return AddUserLimitRate(limit, TimeSpan.FromHours(1), false);
        }

        /// <summary>
        /// Adds the specified <paramref name="headers"/> to the policy.
        /// </summary>
        /// <param name="headers">The headers which need to be allowed in the request.</param>
        /// <returns>The current policy builder</returns>
        public ThrottlingPolicyBuilder AddUserLimitRatePerDay(long limit)
        {
            return AddUserLimitRate(limit, TimeSpan.FromDays(1), false);
        }

        /// <summary>
        /// Adds the specified <paramref name="headers"/> to the policy.
        /// </summary>
        /// <param name="headers">The headers which need to be allowed in the request.</param>
        /// <returns>The current policy builder</returns>
        public ThrottlingPolicyBuilder AddIPLimitRate(long limit, TimeSpan window, bool sliding = false)
        {
            return AddPolicy(new IPLimitRatePolicy(limit, window, sliding));
        }

        /// <summary>
        /// Adds the specified <paramref name="headers"/> to the policy.
        /// </summary>
        /// <param name="headers">The headers which need to be allowed in the request.</param>
        /// <returns>The current policy builder</returns>
        public ThrottlingPolicyBuilder AddIPLimitRatePerHour(long limit)
        {
            return AddIPLimitRate(limit, TimeSpan.FromHours(1), false);
        }

        /// <summary>
        /// Adds the specified <paramref name="headers"/> to the policy.
        /// </summary>
        /// <param name="headers">The headers which need to be allowed in the request.</param>
        /// <returns>The current policy builder</returns>
        public ThrottlingPolicyBuilder AddIPLimitRatePerDay(long limit)
        {
            return AddIPLimitRate(limit, TimeSpan.FromDays(1), false);
        }

        /// <summary>
        /// Adds the specified <paramref name="headers"/> to the policy.
        /// </summary>
        /// <param name="headers">The headers which need to be allowed in the request.</param>
        /// <returns>The current policy builder</returns>
        public ThrottlingPolicyBuilder AddClientLimitRate(long limit, TimeSpan window, bool sliding = false)
        {
            return AddPolicy(new ClientLimitRatePolicy(limit, window, sliding));
        }

        /// <summary>
        /// Adds the specified <paramref name="headers"/> to the policy.
        /// </summary>
        /// <param name="headers">The headers which need to be allowed in the request.</param>
        /// <returns>The current policy builder</returns>
        public ThrottlingPolicyBuilder AddClientLimitRatePerHour(long limit, TimeSpan window)
        {
            return AddIPLimitRate(limit, TimeSpan.FromHours(1), false);
        }

        /// <summary>
        /// Adds the specified <paramref name="headers"/> to the policy.
        /// </summary>
        /// <param name="headers">The headers which need to be allowed in the request.</param>
        /// <returns>The current policy builder</returns>
        public ThrottlingPolicyBuilder AddClientLimitRatePerDay(long limit, TimeSpan window)
        {
            return AddIPLimitRate(limit, TimeSpan.FromDays(1), false);
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
            return _policy;
        }
    }

    public sealed class ThrottlingPolicy : IThrottlingPolicy
    {
        private readonly List<IThrottlingPolicy> _policies = new List<IThrottlingPolicy>();

        public string Category { get; set; }

        public void AddPolicy([NotNull] IThrottlingPolicy policy)
        {
            _policies.Add(policy);
            policy.Category = Category + "_" + policy.Category + "_" + _policies.Count;
        }

        public void Configure(ThrottlingOptions options)
        {
            for (int i = 0; i < _policies.Count; i++)
            {
                _policies[i].Configure(options);
            }
        }

        public async Task<IEnumerable<ThrottlingResult>> EvaluateAsync([NotNull] HttpContext context)
        {
            IEnumerable<ThrottlingResult> results = new List<ThrottlingResult>();
            for (int i = 0; i < _policies.Count; i++)
            {
                var policy = _policies[i];
                var policyResult = await policy.EvaluateAsync(context);
                results = results.Concat(policyResult);
            }

            return results;
        }
    }
}
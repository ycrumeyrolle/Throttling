using System;
using System.Collections.Generic;
using Microsoft.Framework.Internal;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Routing;
using Microsoft.AspNet.Routing.Template;
using System.Linq;
using Throttling.IPRanges;

namespace Throttling
{
    public class ThrottlingOptions
    {
        private string _defaultPolicyName = "__DefaultThrottlingPolicy";

        public string DefaultPolicyName
        {
            get
            {
                return _defaultPolicyName;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                _defaultPolicyName = value;
            }
        }

        public RetryAfterMode RetryAfterMode { get; set; }

        public IRateStore RateStore { get; set; }

        public ISystemClock Clock { get; set; }

        public IClientKeyProvider ClientKeyProvider { get; set; }

        internal void ConfigurePolicies()
        {
            foreach (var policy in Routes.PolicyMap.Values)
            {
                policy.Configure(this);
            }
        }


        /// <summary>
        /// Adds a new policy.
        /// </summary>
        /// <param name="name">The name of the policy.</param>
        /// <param name="policy">The <see cref="IThrottlingPolicy"/> policy to be added.</param>
        public void AddPolicy([NotNull] string name, [NotNull] IThrottlingPolicy policy)
        {
            Routes.PolicyMap[name] = policy;
        }

        /// <summary>
        /// Adds a new policy.
        /// </summary>
        /// <param name="name">The name of the policy.</param>
        /// <param name="configurePolicy">A delegate which can use a policy builder to build a policy.</param>
        public void AddPolicy([NotNull] string name, [NotNull] Action<ThrottlingPolicyBuilder> configurePolicy)
        {
            var policyBuilder = new ThrottlingPolicyBuilder(name);
            configurePolicy(policyBuilder);
            Routes.PolicyMap[name] = policyBuilder.Build();
        }

        /// <summary>
        /// Gets the policy based on the <paramref name="name"/>
        /// </summary>
        /// <param name="name">The name of the policy to lookup.</param>
        /// <returns>The <see cref="ThrottlingPolicy"/> if the policy was added.<c>null</c> otherwise.</returns>
        public IThrottlingPolicy GetPolicy([NotNull] string name)
        {
            return Routes.PolicyMap.ContainsKey(name) ? Routes.PolicyMap[name] : null;
        }

        public IThrottlingRouter Routes { get; set; }

    }
}
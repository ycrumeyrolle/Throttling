using System;
using Microsoft.Framework.Internal;

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
                    throw new ArgumentNullException(nameof(value));
                }

                _defaultPolicyName = value;
            }
        }

        public RetryAfterMode RetryAfterMode { get; set; }

        public IThrottlingRouter Routes { get; set; }

        public bool SendThrottlingHeaders { get; set; }

        /// <summary>
        /// Adds a new policy.
        /// </summary>
        /// <param name="name">The name of the policy.</param>
        /// <param name="policy">The <see cref="IThrottlingPolicy"/> policy to be added.</param>
        public void AddPolicy([NotNull] string name, [NotNull] ThrottlingPolicy policy)
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
        public ThrottlingPolicy GetPolicy([NotNull] string name)
        {
            return Routes.PolicyMap.ContainsKey(name) ? Routes.PolicyMap[name] : null;
        }
    }
}
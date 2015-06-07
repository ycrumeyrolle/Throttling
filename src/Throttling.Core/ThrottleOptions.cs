using System;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public class ThrottleOptions
    {
        private string _defaultPolicyName = "__DefaultThrottlePolicy";

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

        public IThrottleRouter Routes { get; set; }

        public bool SendThrottleHeaders { get; set; }

        /// <summary>
        /// Adds a new policy.
        /// </summary>
        /// <param name="name">The name of the policy.</param>
        /// <param name="policy">The <see cref="IThrottlePolicy"/> policy to be added.</param>
        public void AddPolicy([NotNull] string name, [NotNull] ThrottlePolicy policy)
        {
            Routes.PolicyMap[name] = policy;
        }

        /// <summary>
        /// Adds a new policy.
        /// </summary>
        /// <param name="name">The name of the policy.</param>
        /// <param name="configurePolicy">A delegate which can use a policy builder to build a policy.</param>
        public void AddPolicy([NotNull] string name, [NotNull] Action<ThrottlePolicyBuilder> configurePolicy)
        {
            var policyBuilder = new ThrottlePolicyBuilder(name);
            configurePolicy(policyBuilder);
            Routes.PolicyMap[name] = policyBuilder.Build();
        }

        /// <summary>
        /// Gets the policy based on the <paramref name="name"/>
        /// </summary>
        /// <param name="name">The name of the policy to lookup.</param>
        /// <returns>The <see cref="ThrottlePolicy"/> if the policy was added.<c>null</c> otherwise.</returns>
        public ThrottlePolicy GetPolicy([NotNull] string name)
        {
            return Routes.PolicyMap.ContainsKey(name) ? Routes.PolicyMap[name] : null;
        }
    }
}
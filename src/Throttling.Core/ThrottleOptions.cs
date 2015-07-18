using System;
using System.Collections.Generic;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public class ThrottleOptions
    {
        private string _defaultPolicyName = "__DefaultThrottlePolicy";
        private readonly IList<ThrottlePolicyBuilder> _builders = new List<ThrottlePolicyBuilder>();
        private bool policyBuildCompleted;

        /// <summary>
        /// Gets or sets the default policy name.
        /// </summary>
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

        /// <summary>
        /// Gets or set the mode to generate the Retry-After header.
        /// </summary>
        public RetryAfterMode RetryAfterMode { get; set; }

        internal void BuildPolicies()
        {
            if (!policyBuildCompleted)
            {
                foreach (var builder in _builders)
                {
                    PolicyMap[builder.Name] = builder.Build(this);
                }

                policyBuildCompleted = true;
            }
        }

        /// <summary>
        /// Gets or sets the routes.
        /// </summary>
        public IThrottleRouter Routes { get; set; } = new ThrottleRouteCollection();

        /// <summary>
        /// Gets or sets a value indicating whether the throttling headers will be sent.
        /// </summary>
        public bool SendThrottleHeaders { get; set; }

        public IDictionary<string, ThrottlePolicy> PolicyMap { get; } = new Dictionary<string, ThrottlePolicy>();

        /// <summary>
        /// Adds a new policy.
        /// </summary>
        /// <param name="name">The name of the policy.</param>
        /// <param name="policy">The <see cref="IThrottlePolicy"/> policy to be added.</param>
        public void AddPolicy([NotNull] string name, [NotNull] ThrottlePolicy policy)
        {
            PolicyMap[name] = policy;
        }

        /// <summary>
        /// Adds a new policy.
        /// </summary>
        /// <param name="name">The name of the policy.</param>
        public ThrottlePolicyBuilder AddPolicy([NotNull] string name)
        {
            var policyBuilder = new ThrottlePolicyBuilder(name);
            _builders.Add(policyBuilder);
            return policyBuilder;
        }

        /// <summary>
        /// Gets the policy based on the <paramref name="name"/>
        /// </summary>
        /// <param name="name">The name of the policy to lookup.</param>
        /// <returns>The <see cref="ThrottlePolicy"/> if the policy was added.<c>null</c> otherwise.</returns>
        public ThrottlePolicy GetPolicy([NotNull] string name)
        {
            return PolicyMap.ContainsKey(name) ? PolicyMap[name] : null;
        }
    }
}
using System;
using System.Collections.Generic;
using Microsoft.Framework.Internal;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Routing;
using Microsoft.AspNet.Routing.Template;
using System.Linq;

namespace Throttling
{
    public class ThrottlingOptions
    {
        private string _defaultPolicyName = "__DefaultThrottlingPolicy";
        private IDictionary<string, IThrottlingPolicy> PolicyMap { get; } = new Dictionary<string, IThrottlingPolicy>();

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
            foreach (var policy in PolicyMap.Values)
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
            PolicyMap[name] = policy;
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
            PolicyMap[name] = policyBuilder.Build();
        }

        /// <summary>
        /// Gets the policy based on the <paramref name="name"/>
        /// </summary>
        /// <param name="name">The name of the policy to lookup.</param>
        /// <returns>The <see cref="ThrottlingPolicy"/> if the policy was added.<c>null</c> otherwise.</returns>
        public IThrottlingPolicy GetPolicy([NotNull] string name)
        {
            return PolicyMap.ContainsKey(name) ? PolicyMap[name] : null;
        }

        public IThrottlingRouter Routes { get; set; }

        public void ApplyStrategy(IEnumerable<string> httpMethods, [NotNull] string routeTemplate, [NotNull] IThrottlingPolicy policy)
        {
            Routes.Add(new UnnamedThrottlingRoute(httpMethods, routeTemplate, policy));
        }

        public void ApplyStrategy(IEnumerable<string> httpMethods, [NotNull] string routeTemplate, [NotNull] string policyName, [NotNull] Action<ThrottlingPolicyBuilder> configurePolicy)
        {
            var policyBuilder = new ThrottlingPolicyBuilder(policyName);
            configurePolicy(policyBuilder);
            ApplyStrategy(httpMethods, routeTemplate, policyBuilder.Build());
        }

        public void ApplyStrategy(IEnumerable<string> httpMethods, [NotNull] string routeTemplate, [NotNull] string policyName)
        {
            IThrottlingPolicy policy;
            if (!PolicyMap.TryGetValue(policyName, out policy))
            {
                throw new InvalidOperationException("Not policy named '" + policyName + "'");
            }

            ApplyStrategy(httpMethods, routeTemplate, policy);
        }

        public void ApplyStrategy([NotNull] string httpMethod, [NotNull] string routeTemplate, [NotNull] IThrottlingPolicy policy)
        {
            ApplyStrategy(new[] { httpMethod }, routeTemplate, policy);
        }

        public void ApplyStrategy([NotNull] string httpMethod, [NotNull] string routeTemplate, [NotNull] string policyName, [NotNull] Action<ThrottlingPolicyBuilder> configurePolicy)
        {
            ApplyStrategy(new[] { httpMethod }, routeTemplate, policyName, configurePolicy);
        }

        public void ApplyStrategy([NotNull] string httpMethod, [NotNull] string routeTemplate, [NotNull] string policyName)
        {
            ApplyStrategy(new[] { httpMethod }, routeTemplate, policyName);
        }

        public void ApplyStrategy([NotNull] string routeTemplate, [NotNull] IThrottlingPolicy policy)
        {
            ApplyStrategy((IEnumerable<string>)null, routeTemplate, policy);
        }
        public void ApplyStrategy([NotNull] string routeTemplate, [NotNull] string policyName)
        {
            ApplyStrategy((IEnumerable<string>)null, routeTemplate, policyName);
        }

        public void ApplyStrategy([NotNull] string routeTemplate, [NotNull] string policyName, [NotNull] Action<ThrottlingPolicyBuilder> configurePolicy)
        {
            ApplyStrategy((IEnumerable<string>)null, routeTemplate, policyName, configurePolicy);
        }
    }
}
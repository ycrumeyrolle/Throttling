using System;

namespace Throttling
{
    internal class ThrottleNamedPolicyBuilder : IThrottlePolicyBuilder
    {
        private readonly string _policyName;

        public ThrottleNamedPolicyBuilder(string policyName)
        {
            if (policyName == null)
            {
                throw new ArgumentNullException(nameof(policyName));
            }

            _policyName = policyName;
        }

        public ThrottlePolicy Build(ThrottleOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return options.GetPolicy(_policyName);
        }
    }
}
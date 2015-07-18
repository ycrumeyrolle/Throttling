using System;
using Microsoft.Framework.Internal;

namespace Throttling
{
    internal class ThrottleNamedPolicyBuilder : IThrottlePolicyBuilder
    {
        private readonly string _policyName;

        public ThrottleNamedPolicyBuilder(string policyName)
        {
            _policyName = policyName;
        }

        public ThrottlePolicy Build([NotNull]ThrottleOptions options)
        {
            return options.GetPolicy(_policyName);
        }
    }
}
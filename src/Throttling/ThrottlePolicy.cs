using System;
using System.Collections.Generic;
using Microsoft.Extensions.Internal;

namespace Throttling
{
    public sealed class ThrottlePolicy
    {
        public ThrottlePolicy(IList<IThrottleRequirement> requirements, IList<IThrottleExclusion> exclusions, string policyName)
        {
            if (requirements == null)
            {
                throw new ArgumentNullException(nameof(requirements));
            }

            if (exclusions == null)
            {
                throw new ArgumentNullException(nameof(exclusions));
            }

            if (policyName == null)
            {
                throw new ArgumentNullException(nameof(policyName));
            }

            if (requirements.Count == 0)
            {
                throw new ArgumentException("The requirements cannot be empty.", nameof(requirements));
            }

            Requirements = new List<IThrottleRequirement>(requirements).AsReadOnly();
            Exclusions = new List<IThrottleExclusion>(exclusions).AsReadOnly();
            Name = policyName;
        }

        public string Name { get; set; }

        public IReadOnlyList<IThrottleRequirement> Requirements { get; }

        public IReadOnlyList<IThrottleExclusion> Exclusions { get; }
    }
}
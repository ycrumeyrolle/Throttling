using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public sealed class ThrottlePolicy
    {
        public ThrottlePolicy([NotNull] IEnumerable<IThrottleRequirement> requirements,
            [NotNull] IEnumerable<IThrottleExclusion> exclusions,
            [NotNull] string policyName)
        {
            if (requirements.Count() == 0)
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
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public sealed class ThrottlingPolicy
    {
        public ThrottlingPolicy([NotNull] IEnumerable<IThrottlingRequirement> requirements,
            [NotNull] IEnumerable<IThrottlingExclusion> exclusions,
            [NotNull] string policyName)
        {
            if (requirements.Count() == 0)
            {
                throw new ArgumentException("The requirements cannot be empty.", nameof(requirements));
            }

            Requirements = new List<IThrottlingRequirement>(requirements).AsReadOnly();
            Exclusions = new List<IThrottlingExclusion>(exclusions).AsReadOnly();
            Name = policyName;
        }

        public string Name
        {
            get; set;
        }


        public IReadOnlyList<IThrottlingRequirement> Requirements
        {
            get;
        }
        public IReadOnlyList<IThrottlingExclusion> Exclusions
        {
            get;
        }
    }
}
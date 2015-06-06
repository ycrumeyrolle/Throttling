using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Framework.Internal;
using Throttling.IPRanges;

namespace Throttling
{
    public sealed class ThrottlingPolicy
    {
        public ThrottlingPolicy([NotNull] IEnumerable<IThrottlingRequirement> requirements, [NotNull] IEnumerable<IPAddressRange> whitelist, [NotNull] string policyName)
        {
            if (requirements.Count() == 0)
            {
                throw new ArgumentException("The argument cannot be empty.", nameof(requirements));
            }

            Requirements = new List<IThrottlingRequirement>(requirements).AsReadOnly();
            Whitelist = new IPWhitelist(whitelist);
            Name = policyName;
        }

        public string Name { get; set; }


        public IReadOnlyList<IThrottlingRequirement> Requirements { get; private set; }

        public IPWhitelist Whitelist { get; private set; }
    }
}
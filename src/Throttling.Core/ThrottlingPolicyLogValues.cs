using Microsoft.Framework.Internal;
using Microsoft.Framework.Logging;

namespace Throttling
{
    public class ThrottlingPolicyLogValues : ReflectionBasedLogValues
    {
        string Name { get; set; }

        string Category { get; set; }

        public ThrottlingPolicyLogValues([NotNull] ThrottlingPolicy inner)
        {
            Name = inner.Name;
        }
    }
}
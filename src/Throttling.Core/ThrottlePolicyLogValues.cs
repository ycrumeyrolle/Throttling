using Microsoft.Framework.Internal;
using Microsoft.Framework.Logging;

namespace Throttling
{
    public class ThrottlePolicyLogValues : ReflectionBasedLogValues
    {
        string Name { get; set; }

        string Category { get; set; }

        public ThrottlePolicyLogValues([NotNull] ThrottlePolicy inner)
        {
            Name = inner.Name;
        }
    }
}
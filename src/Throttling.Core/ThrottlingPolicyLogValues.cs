using Microsoft.Framework.Internal;
using Microsoft.Framework.Logging;

namespace Throttling
{
    public class ThrottlingPolicyLogValues : ReflectionBasedLogValues
    {
        string Name { get; set; }

        string Category { get; set; }

        public ThrottlingPolicyLogValues([NotNull] IThrottlingPolicy inner)
        {
            Name = inner.Name;
            Category = inner.Category;
        }
    }
}
using System.Collections.Generic;
using Microsoft.Framework.Internal;
using Microsoft.Framework.Logging;

namespace Throttling
{
    public class ThrottlePolicyLogValues : ILogValues
    {
        string Name { get; set; }

        string Category { get; set; }

        public ThrottlePolicyLogValues([NotNull] ThrottlePolicy inner)
        {
            Name = inner.Name;
        }

        public IEnumerable<KeyValuePair<string, object>> GetValues()
        {
            yield return new KeyValuePair<string, object>("Name", Name);
            yield return new KeyValuePair<string, object>("Category", Category);
        }
    }
}
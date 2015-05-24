using System.Collections.Generic;

namespace Throttling.Tests
{
    public static class ApiKeys
    {
        public static IEnumerable<object[]> Values
        {
            get
            {
                return new[]
                {
                    new[] { new[] { "apikeyvalue" } },
                    new[] { new[] { "apikeyvalue1", "apikeyvalue2", "apikeyvalue2" } }
                };
            }
        }
    }
}

using System;
using Microsoft.Framework.Internal;
using Microsoft.Framework.Logging;

namespace Throttling
{
    public class ThrottlingResultLogValues : ReflectionBasedLogValues
    {
        public DateTimeOffset? Reset { get; private set; }

        public ThrottlingResultLogValues(DateTimeOffset? reset)
        {
            Reset = reset;
        }
    }
}
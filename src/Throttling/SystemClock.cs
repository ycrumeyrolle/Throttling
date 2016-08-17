
using System;

namespace Throttling
{
    public class SystemClock : ISystemClock
    {
        public DateTimeOffset UtcNow
        {
            get
            {
                DateTime utcNow = DateTime.UtcNow;
                return utcNow.AddMilliseconds(-utcNow.Millisecond);
            }
        }
    }
}
using System;
using System.Threading.Tasks;
using Microsoft.Framework.Caching.Memory;

namespace Throttling
{
    public static class DateTimeOffsetExtensions
    {
        private static DateTimeOffset epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long ToEpoch(this DateTimeOffset dateTime)
        {
            return (dateTime.Ticks - epoch.Ticks) / TimeSpan.TicksPerSecond;
        }
    }
}
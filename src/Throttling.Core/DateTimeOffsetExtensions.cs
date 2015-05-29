using System;

namespace Throttling
{
    public static class DateTimeOffsetExtensions
    {
        private static readonly DateTimeOffset Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static long ToEpoch(this DateTimeOffset dateTime)
        {
            return (dateTime.Ticks - Epoch.Ticks) / TimeSpan.TicksPerSecond;
        }
    }
}
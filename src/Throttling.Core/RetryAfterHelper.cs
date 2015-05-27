using System;
using System.Globalization;
using Microsoft.Framework.Internal;

namespace Throttling
{
    public static class RetryAfterHelper
    {
        public static string GetRetryAfterValue(ISystemClock clock, RetryAfterMode mode, DateTimeOffset? reset)
        {
            if (!reset.HasValue)
            {
                return null;
            }

            switch (mode)
            {
                case RetryAfterMode.HttpDate:
                    return reset.Value.ToString("r");
                case RetryAfterMode.DeltaSeconds:
                    return Convert.ToInt64((reset.Value - clock.UtcNow).TotalSeconds).ToString(CultureInfo.InvariantCulture);
                default:
                    return null;
            }
        }
    }
}

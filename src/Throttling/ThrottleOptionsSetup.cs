using System;
using Microsoft.Framework.OptionsModel;

namespace Throttling
{
    public class ThrottleOptionsSetup : ConfigureOptions<ThrottleOptions>
    {
        public ThrottleOptionsSetup()
            : base(new Action<ThrottleOptions>(ThrottleOptionsSetup.ConfigureOptions))
        {
        }

        /// <summary>
        /// Set the default options
        /// </summary>
        public static void ConfigureOptions(ThrottleOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            options.RetryAfterMode = RetryAfterMode.DeltaSeconds;
            options.SendThrottleHeaders = true;
        }
    }
}
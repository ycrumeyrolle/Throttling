using System;
using Microsoft.Framework.Internal;
using Microsoft.Framework.OptionsModel;

namespace Throttling
{
    public class ThrottleOptionsSetup : ConfigureOptions<ThrottleOptions>
    {
        private readonly IThrottleRouter _router;

        public ThrottleOptionsSetup([NotNull] IThrottleRouter router)
            : base(new Action<ThrottleOptions>(ThrottleOptionsSetup.ConfigureOptions))
        {
            _router = router;
        }

        public override void Configure([NotNull] ThrottleOptions options, string name = "")
        {
            options.Routes = _router;
            base.Configure(options, name);
        }

        /// <summary>
        /// Set the default options
        /// </summary>
        public static void ConfigureOptions([NotNull] ThrottleOptions options)
        {
            options.RetryAfterMode = RetryAfterMode.DeltaSeconds;
            options.SendThrottleHeaders = true;
        }
    }
}
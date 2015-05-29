using System;
using Microsoft.Framework.Internal;
using Microsoft.Framework.OptionsModel;

namespace Throttling
{
    public class ThrottlingOptionsSetup : ConfigureOptions<ThrottlingOptions>
    {
        private readonly IThrottlingRouter _router;

        public ThrottlingOptionsSetup([NotNull] IThrottlingRouter router)
            : base(new Action<ThrottlingOptions>(ThrottlingOptionsSetup.ConfigureMessageOptions))
        {
            _router = router;
        }

        public override void Configure([NotNull] ThrottlingOptions options, string name = "")
        {
            options.Routes = _router;
            base.Configure(options, name);
        }

        /// <summary>
        /// Set the default options
        /// </summary>
        public static void ConfigureMessageOptions([NotNull] ThrottlingOptions options)
        {
            options.RetryAfterMode = RetryAfterMode.DeltaSeconds;
        }
    }
}
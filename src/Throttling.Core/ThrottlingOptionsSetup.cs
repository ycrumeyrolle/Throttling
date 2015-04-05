using System;
using Microsoft.Framework.Internal;
using Microsoft.Framework.OptionsModel;

namespace Throttling
{
    public class ThrottlingOptionsSetup : ConfigureOptions<ThrottlingOptions>
    {
        private readonly ISystemClock _clock;
        private readonly IRateStore _store;
        private readonly IThrottlingRouter _router;

        public ThrottlingOptionsSetup([NotNull] IRateStore store, [NotNull] ISystemClock clock, [NotNull] IThrottlingRouter router) 
            : base(new Action<ThrottlingOptions>(ThrottlingOptionsSetup.ConfigureMessageOptions))
        {
            _store = store;
            _clock = clock;
            _router = router;
        }

        public override void Configure([NotNull] ThrottlingOptions options, string name = "")
        {
            options.RateStore = _store;
            options.Clock = _clock;
            options.Routes = _router;                 
            base.Configure(options, name);
        }

        /// <summary>
        /// Set the default options
        /// </summary>
        public static void ConfigureMessageOptions(ThrottlingOptions options)
        {
            options.RetryAfterMode = RetryAfterMode.DeltaSeconds;
            options.ClientKeyProvider = new ClientKeyProvider();
        }
    }
}
using System;
using Microsoft.Framework.Internal;
using Microsoft.Framework.OptionsModel;

namespace Throttling
{
    public class ThrottlingOptionsSetup : ConfigureOptions<ThrottlingOptions>
    {
        private readonly ISystemClock _clock;
        private readonly IRateStore _store;
        public ThrottlingOptionsSetup([NotNull] IRateStore store, [NotNull] ISystemClock clock) 
            : base(new Action<ThrottlingOptions>(ThrottlingOptionsSetup.ConfigureMessageOptions))
        {
            this._store = store;
            this._clock = clock;
        }
        public override void Configure([NotNull] ThrottlingOptions options, string name = "")
        {
            options.RateStore = this._store;
            options.Clock = this._clock;            
            base.Configure(options, name);
        }
        /// <summary>
        /// Set the default options
        /// </summary>
        public static void ConfigureMessageOptions(ThrottlingOptions options)
        {
            options.RetryAfterMode = RetryAfterMode.DeltaSeconds;
        }
    }
}
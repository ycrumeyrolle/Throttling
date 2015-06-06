using System;

namespace Throttling
{
    public class FormApiKeyBandwidthRequirement : ApiKeyBandwidthRequirement
    {
        public FormApiKeyBandwidthRequirement(long bandwidth, TimeSpan renewalPeriod, bool sliding, string parameter)
            : base (bandwidth, renewalPeriod, sliding, new FormApiKeyProvider(parameter))
        {
        }
    }
}
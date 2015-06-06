using System;

namespace Throttling
{
    public sealed class HeaderApiKeyBandwidthRequirement : ApiKeyBandwidthRequirement
    {
        public HeaderApiKeyBandwidthRequirement(long bandwidth, TimeSpan renewalPeriod, bool sliding, string parameter)
            : base (bandwidth, renewalPeriod, sliding, new HeaderApiKeyProvider(parameter))
        {
        }
    }
}
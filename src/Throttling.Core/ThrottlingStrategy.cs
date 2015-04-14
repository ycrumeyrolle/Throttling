using Throttling.IPRanges;

namespace Throttling
{
    public class ThrottlingStrategy
    {
        public IThrottlingPolicy Policy { get; set; }

        public string RouteTemplate { get; set; }

        public IPWhitelist Whitelist { get; set; }
    }
}
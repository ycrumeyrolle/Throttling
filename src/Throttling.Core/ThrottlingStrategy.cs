using Throttling.IPRanges;

namespace Throttling
{
    public class ThrottlingStrategy
    {
        public ThrottlingPolicy Policy { get; set; }

        public string RouteTemplate { get; set; }
    }
}
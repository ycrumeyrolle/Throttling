using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Throttling.IPRanges
{
    public class IPWhitelist
    {
        private readonly IEnumerable<IPAddressRange> _ranges;

        public IPWhitelist(IEnumerable<IPAddressRange> ranges)
        {
            _ranges = ranges;
        }

        public bool Contains(IPAddress address)
        {
            return _ranges.Any(range => range.Contains(address));
        }
    }
}
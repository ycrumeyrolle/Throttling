using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Throttling.IPRanges
{
    public class IPWhitelist
    {
        private readonly IPAddressRange[] _ranges;

        public IPWhitelist(IEnumerable<IPAddressRange> ranges)
        {
            _ranges = ranges.ToArray();
        }

        public IPWhitelist(IEnumerable<string> ranges)
        {
            _ranges = ranges.Select(range => IPAddressRange.Parse(range)).ToArray();
        }

        public bool Contains(IPAddress address)
        {
            for (int i = 0; i < _ranges.Length; i++)
            {
                if (_ranges[i].Contains(address))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.Framework.Internal;

namespace Throttling.IPRanges
{
    public class BitMaskPattern : IIPAddressPattern
    {
        // Bit mask : "192.168.0.0/255.255.255.0"
        private static readonly Regex BitMaskRangeRegex = new Regex(@"^(?<adr>[\da-f\.:]+)/(?<bitmask>[\da-f\.:]+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public bool TryParse([NotNull] string range, out IIPAddressRangeValidator rangeValidator)
        {
            var bitMaskMatch = BitMaskRangeRegex.Match(range);
            if (bitMaskMatch.Success)
            {
                var baseAdrBytes = IPAddress.Parse(bitMaskMatch.Groups["adr"].Value).GetAddressBytes();
                IEnumerable<byte> maskBytes = IPAddress.Parse(bitMaskMatch.Groups["bitmask"].Value).GetAddressBytes();
                baseAdrBytes = baseAdrBytes.And(maskBytes).ToArray();
                rangeValidator = new IPAddressRangeValidator(new IPAddress(baseAdrBytes), new IPAddress(baseAdrBytes.Or(maskBytes.Not()).ToArray()));
                return true;
            }

            rangeValidator = null;
            return false;
        }
    }
}
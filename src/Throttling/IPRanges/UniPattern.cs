using System;
using System.Net;
using System.Text.RegularExpressions;

namespace Throttling.IPRanges
{
    public class UniPattern : IIPAddressPattern
    {
        // Uni address: "127.0.0.1", ":;1"
        private static readonly Regex UniAddressRegex = new Regex(@"^(?<adr>[\da-f\.:]+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public bool TryParse(string range, out IIPAddressRangeValidator rangeValidator)
        {
            if (range == null)
            {
                throw new ArgumentNullException(nameof(range));
            }

            var uniMatch = UniAddressRegex.Match(range);
            if (uniMatch.Success)
            {
                var address = IPAddress.Parse(range);
                rangeValidator = new IPSingleAddress(address);

                return true;
            }

            rangeValidator = null;
            return false;
        }
    }
}
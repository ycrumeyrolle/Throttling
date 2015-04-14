using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.Framework.Internal;

namespace Throttling.IPRanges
{
    public class UniPattern : IIPAddressPattern
    {
        // Uni address: "127.0.0.1", ":;1"
        private static readonly Regex UniAddressRegex = new Regex(@"^(?<adr>[\da-f\.:]+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public bool TryParse([NotNull] string range, out IIPAddressRangeValidator rangeValidator)
        {
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
using System;
using System.Net;
using System.Text.RegularExpressions;

namespace Throttling.IPRanges
{
    public class BeginEndPattern : IIPAddressPattern
    {
        // Begin end range: "169.258.0.0-169.258.0.255"
        private static readonly Regex BeginEndRangeRegex = new Regex(@"^(?<begin>[\da-f\.:]+)-(?<end>[\da-f\.:]+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public bool TryParse(string range, out IIPAddressRangeValidator rangeValidator)
        {
            if (range == null)
            {
                throw new ArgumentNullException(nameof(range));
            }

            var beginEndMatch = BeginEndRangeRegex.Match(range);
            if (beginEndMatch.Success)
            {
                rangeValidator = new IPAddressRangeValidator(IPAddress.Parse(beginEndMatch.Groups["begin"].Value), IPAddress.Parse(beginEndMatch.Groups["end"].Value));
                return true;
            }

            rangeValidator = null;
            return false;
        }
    }
}
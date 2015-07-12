using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.Framework.Internal;

namespace Throttling.IPRanges
{
    public class UniListPattern : IIPAddressPattern
    {
        // Uni address: "127.0.0.1", ":;1"
        private static readonly Regex UniAddressRegex = new Regex(@"^(?<adr>[\da-f\.:]+)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public bool TryParse([NotNull] string range, out IIPAddressRangeValidator rangeValidator)
        {            
            var addresses = range.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            var list = new List<IPAddress>();
            foreach (string address in addresses)
            {
                var match = UniAddressRegex.Match(address);
                if (match.Success)
                {
                    list.Add(IPAddress.Parse(match.Groups["adr"].Value));
                }
            }

            if (list.Count > 0)
            {
                rangeValidator = new IPAddressList(list);
                return true;
            }

            rangeValidator = null;
            return false;
        }
    }
}
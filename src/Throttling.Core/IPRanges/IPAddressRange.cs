namespace Throttling.IPRanges
{
    using System;
    using System.Net;
    using Microsoft.Framework.Internal;

    public class IPAddressRange
    {
        private static readonly IIPAddressPattern[] Patterns = 
        {
            new UniListPattern(),
            new UniPattern(),
            new BitMaskPattern(),
            new CidrPattern(),
            new BeginEndPattern()
        };

        private readonly IIPAddressRangeValidator _validator;

        public IPAddressRange([NotNull] string range)
        {
            _validator = ParseCore(range);
        }
        public IPAddressRange([NotNull] IIPAddressRangeValidator rangeValidator)
        {
            _validator = rangeValidator;
        }

        public bool Contains([NotNull] IPAddress address)
        {
            return _validator.Contains(address);
        }

        public bool Contains([NotNull] string address)
        {
            return _validator.Contains(IPAddress.Parse(address));
        }

        public static IPAddressRange Parse([NotNull] string range)
        {
            IPAddressRange addressRange;
            if (!TryParse(range, out addressRange))
            {
                throw new FormatException("Unknown IP range string.");
            }

            return addressRange;
        }

        private static IIPAddressRangeValidator ParseCore([NotNull] string range)
        {
            IIPAddressRangeValidator addressRangeValidator;
            if (!TryParseCore(range, out addressRangeValidator))
            {
                throw new FormatException("Unknown IP range string.");
            }

            return addressRangeValidator;
        }

        public static bool TryParse([NotNull] string range, out IPAddressRange addressRange)
        {
            IIPAddressRangeValidator rangeValidator;
            if (TryParseCore(range, out rangeValidator))
            {
                addressRange = new IPAddressRange(rangeValidator);
                return true;
            }

            addressRange = null;
            return false;
        }

        private static bool TryParseCore(string range, out IIPAddressRangeValidator rangeValidator)
        {
            // remove all spaces.
            range = range.Replace(" ", string.Empty);

            foreach (var pattern in Patterns)
            {
                if (pattern.TryParse(range, out rangeValidator))
                {
                    return true;
                }
            }

            rangeValidator = null;
            return false;
        }
    }
}

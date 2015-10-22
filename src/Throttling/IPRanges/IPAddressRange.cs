namespace Throttling.IPRanges
{
    using System;
    using System.Net;

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

        public IPAddressRange(string range)
        {
            if (range == null)
            {
                throw new ArgumentNullException(nameof(range));
            }

            _validator = ParseCore(range);
        }
        public IPAddressRange(IIPAddressRangeValidator rangeValidator)
        {
            if (rangeValidator == null)
            {
                throw new ArgumentNullException(nameof(rangeValidator));
            }

            _validator = rangeValidator;
        }

        public bool Contains(IPAddress address)
        {
            if (address == null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            return _validator.Contains(address);
        }

        public bool Contains(string address)
        {
            if (address == null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            return _validator.Contains(IPAddress.Parse(address));
        }

        public static IPAddressRange Parse(string range)
        {
            if (range == null)
            {
                throw new ArgumentNullException(nameof(range));
            }

            IPAddressRange addressRange;
            if (!TryParse(range, out addressRange))
            {
                throw new FormatException("Unknown IP range string.");
            }

            return addressRange;
        }

        private static IIPAddressRangeValidator ParseCore(string range)
        {
            if (range == null)
            {
                throw new ArgumentNullException(nameof(range));
            }

            IIPAddressRangeValidator addressRangeValidator;
            if (!TryParseCore(range, out addressRangeValidator))
            {
                throw new FormatException("Unknown IP range string.");
            }

            return addressRangeValidator;
        }

        public static bool TryParse(string range, out IPAddressRange addressRange)
        {
            if (range == null)
            {
                throw new ArgumentNullException(nameof(range));
            }

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

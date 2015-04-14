namespace Throttling.IPRanges
{
    using System;
    using System.Net;
    using Microsoft.Framework.Internal;

    public class IPAddressRange
    {
        private static readonly IIPAddressPattern[] Patterns = new IIPAddressPattern[]
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
            _validator = Parse(range);
        }

        public bool Contains([NotNull] IPAddress address)
        {
            return _validator.Contains(address);
        }

        public bool Contains([NotNull] string address)
        {
            return _validator.Contains(IPAddress.Parse(address));
        }

        public static IIPAddressRangeValidator Parse([NotNull] string range)
        {
            IIPAddressRangeValidator rangeValidator;
            if (!TryParse(range, out rangeValidator))
            {
                throw new FormatException("Unknown IP range string.");
            }

            return rangeValidator;
        }

        public static bool TryParse([NotNull] string range, out IIPAddressRangeValidator rangeValidator)
        {
            // remove all spaces.
            range = range.Replace(" ", string.Empty);

            for (int i = 0; i < Patterns.Length; i++)
            {
                if (Patterns[i].TryParse(range, out rangeValidator))
                {
                    return true;
                }
            }

            rangeValidator = null;
            return false;
        }
    }
}

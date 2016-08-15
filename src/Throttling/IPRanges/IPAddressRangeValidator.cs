namespace Throttling.IPRanges
{
    using System;
    using System.Net;

    public class IPAddressRangeValidator : IIPAddressRangeValidator
    {
        private readonly IPAddress _begin;

        private readonly IPAddress _end;

        public IPAddressRangeValidator(IPAddress begin, IPAddress end)
        {
            if (begin == null)
            {
                throw new ArgumentNullException(nameof(begin));
            }

            if (end == null)
            {
                throw new ArgumentNullException(nameof(end));
            }

            _begin = begin;
            _end = end;
        }

        public bool Contains(IPAddress address)
        {
            if (address == null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            if (address.AddressFamily != _begin.AddressFamily)
            {
                return false;
            }

            var addrBytes = address.GetAddressBytes();
            return _begin.GetAddressBytes().GreatorOrEqual(addrBytes) && _end.GetAddressBytes().LessOrEqual(addrBytes);
        }
    }
}
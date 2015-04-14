namespace Throttling.IPRanges
{
    using System;
    using System.Net;
    using Microsoft.Framework.Internal;

    internal class IPAddressRangeValidator : IIPAddressRangeValidator
    {
        private readonly IPAddress _begin;

        private readonly IPAddress _end;

        public IPAddressRangeValidator([NotNull] IPAddress begin, [NotNull] IPAddress end)
        {
            _begin = begin;
            _end = end;
        }

        public bool Contains([NotNull] IPAddress address)
        {
            if (address.AddressFamily != _begin.AddressFamily)
            {
                return false;
            }

            var addrBytes = address.GetAddressBytes();
            return _begin.GetAddressBytes().GreatorOrEqual(addrBytes) && _end.GetAddressBytes().LessOrEqual(addrBytes);
        }
    }
}
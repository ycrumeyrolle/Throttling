namespace Throttling.IPRanges
{
    using System;
    using System.Collections.Generic;
    using System.Net;

    public class IPAddressList : IIPAddressRangeValidator
    {
        private readonly ICollection<IPAddress> _addresses;

        public IPAddressList(ICollection<IPAddress> addresses)
        {
            if (addresses == null)
            {
                throw new ArgumentNullException(nameof(addresses));
            }

            _addresses = addresses;
        }

        public bool Contains(IPAddress address)
        {
            if (address == null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            return _addresses.Contains(address);
        }
    }
}
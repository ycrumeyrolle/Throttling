namespace Throttling.IPRanges
{
    using System;
    using System.Net;

    public class IPSingleAddress : IIPAddressRangeValidator
    {
        private readonly IPAddress _singleAddress;

        public IPSingleAddress(IPAddress address)
        {
            if (address == null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            _singleAddress = address;
        }

        public bool Contains(IPAddress address)
        {
            if (address == null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            return _singleAddress.Equals(address);
        }
    }
}
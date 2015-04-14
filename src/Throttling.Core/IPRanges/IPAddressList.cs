namespace Throttling.IPRanges
{
    using System.Collections.Generic;
    using System.Net;
    using Microsoft.Framework.Internal;

    public class IPAddressList : IIPAddressRangeValidator
    {
        private readonly ICollection<IPAddress> _addresses;

        public IPAddressList([NotNull] ICollection<IPAddress> addresses)
        {
            _addresses = addresses;
        }

        public bool Contains([NotNull] IPAddress address)
        {
            return _addresses.Contains(address);
        }
    }
}
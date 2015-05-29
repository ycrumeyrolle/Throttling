namespace Throttling.IPRanges
{
    using System.Net;
    using Microsoft.Framework.Internal;

    public class IPSingleAddress : IIPAddressRangeValidator
    {
        private readonly IPAddress _singleAddress;

        public IPSingleAddress([NotNull] IPAddress address)
        {
            _singleAddress = address;
        }

        public bool Contains([NotNull] IPAddress address)
        {
            return _singleAddress.Equals(address);
        }
    }
}
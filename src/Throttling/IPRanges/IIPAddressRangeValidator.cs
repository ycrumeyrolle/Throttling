namespace Throttling.IPRanges
{
    using System.Net;

    public interface IIPAddressRangeValidator
    {
        bool Contains(IPAddress address);
    }
}
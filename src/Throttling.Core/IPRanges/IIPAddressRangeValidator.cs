namespace Throttling.IPRanges
{
    using System.Net;
    using Microsoft.Framework.Internal;

    public interface IIPAddressRangeValidator
    {
        bool Contains([NotNull] IPAddress address);
    }
}
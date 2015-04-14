using Microsoft.Framework.Internal;

namespace Throttling.IPRanges
{
    public interface IIPAddressPattern
    {
        bool TryParse([NotNull] string range, out IIPAddressRangeValidator rangeValidator);
    }
}
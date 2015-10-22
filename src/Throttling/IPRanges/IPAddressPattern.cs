namespace Throttling.IPRanges
{
    public interface IIPAddressPattern
    {
        bool TryParse(string range, out IIPAddressRangeValidator rangeValidator);
    }
}
namespace Throttling.IPRanges
{
    using System.Collections.Generic;
    using System.Linq;

    public static class EnumerableOfByteExtensions
    {
        public static IEnumerable<byte> Not(this IEnumerable<byte> bytes)
        {
            return bytes.Select(b => (byte)~b);
        }

        public static IEnumerable<byte> And(this IEnumerable<byte> left, IEnumerable<byte> right)
        {
            return left.Zip(right, (a, b) => (byte)(a & b));
        }

        public static IEnumerable<byte> Or(this IEnumerable<byte> left, IEnumerable<byte> right)
        {
            return left.Zip(right, (a, b) => (byte)(a | b));
        }

        public static bool GreatorOrEqual(this IEnumerable<byte> left, IEnumerable<byte> right)
        {
            return left.Zip(right, (a, b) => a == b ? 0 : a < b ? 1 : -1)
                .SkipWhile(c => c == 0)
                .FirstOrDefault() >= 0;
        }

        public static bool LessOrEqual(this IEnumerable<byte> left, IEnumerable<byte> right)
        {
            return left.Zip(right, (a, b) => a == b ? 0 : a < b ? 1 : -1)
                .SkipWhile(c => c == 0)
                .FirstOrDefault() <= 0;
        }
    }
}

using System.Linq;

namespace Throttling.IPRanges
{
    public static class BitMaskHelper
    {
        public static byte[] GetBitMask(int sizeOfBuff, int bitLen)
        {
            var maskBytes = new byte[sizeOfBuff];
            var bytesLen = bitLen / 8;
            var bitsLen = bitLen % 8;
            for (int i = 0; i < bytesLen; i++)
            {
                maskBytes[i] = 0xff;
            }

            if (bitsLen > 0)
            {
                maskBytes[bytesLen] = (byte)~Enumerable.Range(1, 8 - bitsLen).Select(n => 1 << n - 1).Aggregate((a, b) => a | b);
            }

            return maskBytes;
        }
    }
}
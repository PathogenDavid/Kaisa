using System;
using System.Buffers.Binary;

namespace Kaisa
{
    internal static class BitHelper
    {
        public static ulong FromBigEndian(ulong x)
            => BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(x) : x;

        public static uint FromBigEndian(uint x)
            => BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(x) : x;

        public static ushort FromBigEndian(ushort x)
            => BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(x) : x;
    }
}

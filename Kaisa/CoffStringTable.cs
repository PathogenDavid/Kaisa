using System;
using System.IO;
using System.Text;

namespace Kaisa
{
    public readonly struct CoffStringTable
    {
        private readonly byte[] StringTable;

        public CoffStringTable(Stream stream)
        {
            uint stringTableSize = stream.Read<uint>();
            stream.Position -= sizeof(uint); // Offsets and the size include the size field
            StringTable = stream.Read<byte>(checked((int)stringTableSize));

            if (stringTableSize > sizeof(uint) && StringTable[stringTableSize - 1] != 0)
            { throw new ArgumentException("The string table is malformed.", nameof(stream)); }
        }

        public string GetString(int offset)
        {
            if (offset < 0 || offset >= StringTable.Length)
            { throw new ArgumentOutOfRangeException(nameof(offset)); }

            int end = offset;
            for (; end < StringTable.Length && StringTable[end] != 0; end++)
            { }

            int length = end - offset;
            return Encoding.ASCII.GetString(StringTable, offset, length);
        }
    }
}

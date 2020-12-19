using System.IO;

namespace Kaisa
{
    // https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#section-table-section-headers
    public struct SectionHeader
    {
        public string Name { get; }
        public uint VirtualSize { get; }
        public uint VirtualAddress { get; }
        public uint SizeOfRawData { get; }
        public uint PointerToRawData { get; }
        public uint PointerToRelocations { get; }
        public uint PointerToLinenumbers { get; }
        public ushort NumberOfRelocations { get; }
        public ushort NumberOfLinenumbers { get; }
        public SectionFlags Characteristics { get; }

        public SectionHeader(Stream stream)
        {
            Name = stream.ReadUtf8(8).TrimEnd('\0');
            VirtualSize = stream.Read<uint>();
            VirtualAddress = stream.Read<uint>();
            SizeOfRawData = stream.Read<uint>();
            PointerToRawData = stream.Read<uint>();
            PointerToRelocations = stream.Read<uint>();
            PointerToLinenumbers = stream.Read<uint>();
            NumberOfRelocations = stream.Read<ushort>();
            NumberOfLinenumbers = stream.Read<ushort>();
            Characteristics = stream.Read<SectionFlags>();
        }

        public override string ToString()
            => $"'{Name}' @ {VirtualAddress} for {VirtualSize} bytes, Characteristics = {Characteristics}";
    }
}

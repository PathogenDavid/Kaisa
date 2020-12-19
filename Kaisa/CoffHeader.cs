using System;
using System.IO;

namespace Kaisa
{
    // https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#coff-file-header-object-and-image
    public struct CoffHeader
    {
        public ImageFileMachineType Machine { get; }
        public ushort NumberOfSections { get; }
        public uint TimeDateStamp { get; }
        public uint PointerToSymbolTable { get; }
        public uint NumberOfSymbols { get; }
        public ushort SizeOfOptionalHeader { get; }
        public ImageFileCharacteristics Characteristics { get; }

        public CoffHeader(Stream stream)
            : this(stream.Read<ImageFileMachineType>(), stream.Read<ushort>(), stream)
        { }

        internal CoffHeader(ushort sig1, ushort sig2, Stream stream)
            : this((ImageFileMachineType)sig1, sig2, stream)
        { }

        private CoffHeader(ImageFileMachineType machine, ushort numberOfSections, Stream stream)
        {
            if (machine == ImageFileMachineType.Unknown && numberOfSections == 0xFFFF)
            { throw new ArgumentException("The header describes an import object."); }

            Machine = machine;
            NumberOfSections = numberOfSections;
            TimeDateStamp = stream.Read<uint>();
            PointerToSymbolTable = stream.Read<uint>();
            NumberOfSymbols = stream.Read<uint>();
            SizeOfOptionalHeader = stream.Read<ushort>();
            Characteristics = stream.Read<ImageFileCharacteristics>();
        }
    }
}

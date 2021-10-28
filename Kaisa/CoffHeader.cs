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
            { throw new MalformedFileException("The header describes an import object.", stream.Position - (sizeof(ushort) * 2)); }

            Machine = machine;
            NumberOfSections = numberOfSections;
            TimeDateStamp = stream.Read<uint>();
            PointerToSymbolTable = stream.Read<uint>();
            NumberOfSymbols = stream.Read<uint>();
            SizeOfOptionalHeader = stream.Read<ushort>();
            Characteristics = stream.Read<ImageFileCharacteristics>();
        }

        /// <summary>Checks if the specified stream contains what appears to have a COFF object.</summary>
        /// <remarks>
        /// COFF files don't have any recognizable header magic, so we guess if a file is a COFF file based on whether we recognize the machine type.
        /// </remarks>
        public static bool IsMaybeCoffObject(Stream stream)
        {
            if (!stream.CanSeek)
            { throw new NotSupportedException("Cannot check streams that are not seekable."); }

            long oldPosition = stream.Position;
            ushort sig1 = stream.Read<ushort>();
            ushort sig2 = stream.Read<ushort>();
            stream.Position = oldPosition;
            return IsMaybeCoffObject(sig1, sig2);
        }

        internal static bool IsMaybeCoffObject(ushort sig1, ushort sig2)
            // COFF files don't have any recognizable header magic, so we can only guess if a file is a COFF file based on whether we recognize the machine type
            => Enum.IsDefined((ImageFileMachineType)sig1);
    }
}

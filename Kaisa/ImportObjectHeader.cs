using System;
using System.Diagnostics;
using System.IO;

namespace Kaisa
{
    // https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#import-header
    public struct ImportObjectHeader
    {
        public ushort Version { get; }
        public ImageFileMachineType Machine { get; }
        public uint TimeDateStamp { get; }
        public uint SizeOfData { get; }
        public ushort OrdinalOrHint { get; }
        public ImportType Type { get; }
        public ImportNameType NameType { get; }

        public ImportObjectHeader(Stream stream)
            : this(stream.Read<ushort>(), stream.Read<ushort>(), stream)
        { }

        internal ImportObjectHeader(ushort sig1, ushort sig2, Stream stream)
        {
            if (!IsImportObject(sig1, sig2))
            { throw new ArgumentException("The import object signature is invalid.", nameof(sig1)); }

            Version = stream.Read<ushort>();
            Machine = stream.Read<ImageFileMachineType>();
            TimeDateStamp = stream.Read<uint>();
            SizeOfData = stream.Read<uint>();
            OrdinalOrHint = stream.Read<ushort>();

            ushort bitfield = stream.Read<ushort>();

            Type = (ImportType)(bitfield & 0b11);
            NameType = (ImportNameType)((bitfield >> 2) & 0b111);
            Debug.Assert((bitfield >> 5) == 0, "The reserved bits are expected to be 0");
        }

        public static bool IsImportObject(ushort sig1, ushort sig2)
            => sig1 == (ushort)ImageFileMachineType.Unknown && sig2 == 0xFFFF;
    }
}

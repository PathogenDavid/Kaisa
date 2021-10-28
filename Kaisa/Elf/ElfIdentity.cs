using System;
using System.Diagnostics;
using System.IO;

namespace Kaisa.Elf
{
    // https://refspecs.linuxfoundation.org/elf/gabi4+/ch4.eheader.html -- See "ELF Identification"
    internal readonly struct ElfIdentity
    {
        internal const int EI_NIDENT = 16;

        public bool Is64Bit { get; }
        public ElfOperatingSystemAbi OperatingSystemAbi { get; }
        public byte OperatingSystemAbiVersion { get; }

        public ElfIdentity(Stream stream)
            : this(stream.Read<ushort>(), stream.Read<ushort>(), stream)
        { }

        internal ElfIdentity(ushort sig1, ushort sig2, Stream stream)
        {
            long identityStartOffset = stream.Position - (sizeof(ushort) * 2);

            // EI_MAG0, EI_MAG1, EI_MAG2, EI_MAG3 -- File identification
            if (!IsElfFile(sig1, sig2))
            { throw new MalformedFileException("The ELF magic is invalid.", identityStartOffset); }

            // EI_CLASS -- File class
            byte fileClass = stream.Read<byte>();
            Is64Bit = fileClass switch
            {
                // ELFCLASS32
                1 => false,
                // ELFCLASS64
                2 => true,
                _ => throw new MalformedFileException($"The specified ELF file has an invalid class '{fileClass}'.", stream.Position - 1)
            };

            // EI_DATA -- Data encoding
            // "Primarily for the convenience of code that looks at the ELF file at runtime, the ELF data structures are
            //  intended to have the same byte order as that of the running program."
            // As such there is not a realistic expectation we'll ever need to handle big endian files.
            // (Kaisa in general only supports little endian systems, but since in theory this value matching the system endianess makes us correct we word the errors as such.)
            byte dataEncoding = stream.Read<byte>();
            switch (dataEncoding)
            {
                // ELFDATA2LSB
                case 1:
                    if (!BitConverter.IsLittleEndian)
                    { throw new NotSupportedException("Cannot parse ELF files encoded in little-endian on big-endian systems."); }
                    break;
                // ELFDATA2MSB
                case 2:
                    if (BitConverter.IsLittleEndian)
                    { throw new NotSupportedException("Cannot parse ELF files encoded in big-endian on little-endian systems."); }
                    break;
                default:
                    throw new MalformedFileException($"The specified ELF file has an invalid data encoding '{dataEncoding}'.", stream.Position - 1);
            };

            // EI_VERSION -- File version
            byte version = stream.Read<byte>();
            // This is the only valid value here: "Currently, this value must be EV_CURRENT" (EV_CURRENT is 1)
            if (version != 1)
            { throw new MalformedFileException($"The ELF file has an invalid or unrecognized version '{version}'.", stream.Position - 1); }

            // EI_OSABI -- Operating system/ABI identification
            OperatingSystemAbi = stream.Read<ElfOperatingSystemAbi>();

            // EI_ABIVERSION -- ABI version
            OperatingSystemAbiVersion = stream.Read<byte>();

            // EI_PAD -- Padding bytes
            stream.Position += EI_NIDENT - 9;
            Debug.Assert(stream.Position == identityStartOffset + EI_NIDENT);
        }

        public static bool IsElfFile(Stream stream)
        {
            if (!stream.CanSeek)
            { throw new NotSupportedException("Cannot check streams that are not seekable."); }

            long oldPosition = stream.Position;
            Span<ushort> signature = stackalloc ushort[2];
            bool success = stream.TryRead(signature);
            stream.Position = oldPosition;

            return success && IsElfFile(signature[0], signature[1]);
        }

        internal static bool IsElfFile(ushort sig1, ushort sig2)
            // 0x7F, 'E', 'L', 'F'
            => sig1 == 0x457F && sig2 == 0x464C;
    }
}

using System;
using System.Diagnostics;
using System.IO;

namespace Kaisa.Elf
{
    public readonly struct ElfHeader
    {
        private readonly ElfIdentity Identity;
        public bool Is64Bit => Identity.Is64Bit;
        public ElfOperatingSystemAbi OperatingSystemAbi => Identity.OperatingSystemAbi;
        public byte OperatingSystemAbiVersion => Identity.OperatingSystemAbiVersion;

        public ElfType Type { get; }
        public ElfMachine Machine { get; }
        public uint FileFormatVersion { get; }
        public ulong EntryPointVirtualAddress { get; }
        public long ProgramHeaderTableFileOffset { get; }
        public long SectionHeaderTableFileOffset { get; }
        /// <summary>The length of the program header table, 0 if the file has no program header table.</summary>
        public ushort ProgramHeaderTableLength { get; }
        /// <summary>The length of the section header table, see remarks on the meaning of 0.</summary>
        /// <remarks>
        /// If this value is 0, the file either has no section header table or the number of sections >= 0xFF00.
        /// In the case of the number of sections >= 0xFF00 the number of section header table entries is the size field of the section header at index 0.
        /// Otherwise the size of the section header at index 0 is always 0.
        /// </remarks>
        public ushort SectionHeaderTableLength { get; }
        /// <summary>The index of the string table, see remarks for special values.</summary>
        /// <remarks>
        /// If this value is 0, the file does not contain a string table.
        ///
        /// If this value is <see cref="ushort.MaxValue"/>, then the index is >= 0xFF00 and the actual index is found in <see cref="ElfSectionHeader.Link"/> of the 0th section header.
        /// </remarks>
        public ushort StringTableSectionIndex { get; }

        // These are not used by x64: https://refspecs.linuxfoundation.org/elf/x86_64-abi-0.99.pdf#page=61
        public uint ProcessorSpecificFlags { get; }

        public ElfHeader(Stream stream)
            : this(new ElfIdentity(stream), stream)
        { }

        internal ElfHeader(ElfIdentity identity, Stream stream)
        {
            long elfHeaderStart = stream.Position - ElfIdentity.EI_NIDENT;

            // e_ident
            Identity = identity;

            // Utility functions for reading Elf32_Addr/Elf64_Addr and Elf32_Off/Elf64_Off
            // https://refspecs.linuxfoundation.org/elf/gabi4+/ch4.intro.html
            bool is64Bit = Identity.Is64Bit;
            ulong ReadAddress()
                => is64Bit ? stream.Read<ulong>() : stream.Read<uint>();

            long ReadOffset()
                => checked((long)ReadAddress());

            // Half e_type;
            Type = stream.Read<ElfType>();

            // Half e_machine;
            Machine = stream.Read<ElfMachine>();

            // Word e_version;
            FileFormatVersion = stream.Read<uint>();
            if (FileFormatVersion != 1)
            { throw new NotSupportedException($"ELF file version '{FileFormatVersion}' is not supported."); }

            // Addr e_entry;
            EntryPointVirtualAddress = ReadAddress();
            // Off e_phoff;
            ProgramHeaderTableFileOffset = ReadOffset();
            // Off e_shoff;
            SectionHeaderTableFileOffset = ReadOffset();

            // Word e_flags;
            ProcessorSpecificFlags = stream.Read<uint>();

            // Half e_ehsize;
            ushort headerSize = stream.Read<ushort>();
            ushort expectedHeaderSize = is64Bit ? (ushort)64 : (ushort)52;
            if (headerSize != expectedHeaderSize)
            {
                throw new MalformedFileException
                (
                    $"Size recorded in ELF header ({headerSize}) does not match expected header size of {expectedHeaderSize} bytes.",
                    stream.Position - sizeof(ushort)
                );
            }

            // Half e_phentsize;
            ushort programHeaderSize = stream.Read<ushort>();
            //TODO: Validate

            // Half e_phnum;
            ProgramHeaderTableLength = stream.Read<ushort>();

            // Half e_shentsize;
            ushort sectionHeaderSize = stream.Read<ushort>();
            ushort expectedSectionHeaderSize = is64Bit ? ElfSectionHeader.Size64 : ElfSectionHeader.Size32;
            if (sectionHeaderSize != expectedSectionHeaderSize)
            {
                throw new MalformedFileException
                (
                    $"Size recorded in ELF header ({sectionHeaderSize}) does not match expected section header size of {expectedSectionHeaderSize} bytes.",
                    stream.Position - sizeof(ushort)
                );
            }

            // Half e_shnum;
            SectionHeaderTableLength = stream.Read<ushort>();

            // Half e_shstrndx;
            StringTableSectionIndex = stream.Read<ushort>();

            Debug.Assert(stream.Position == elfHeaderStart + expectedHeaderSize);
        }

        public static bool IsElfFile(Stream stream)
            => ElfIdentity.IsElfFile(stream);
    }
}

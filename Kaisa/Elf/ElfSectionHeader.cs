﻿using System.IO;

namespace Kaisa.Elf
{
    // https://refspecs.linuxfoundation.org/elf/gabi4+/ch4.sheader.html
    public readonly struct ElfSectionHeader
    {
        internal const ushort Size32 = sizeof(int) * 10;
        internal const ushort Size64 = sizeof(int) * 4 + sizeof(long) * 6;

        internal long HeaderStart { get; }
        public uint NameIndex { get; }

        public ElfSectionType Type { get; }
        public bool IsOperatingSystemSpecific => Type >= ElfSectionType.FirstOperatingSystemSpecificSection && Type <= ElfSectionType.LastOperatingSystemSpecificSection;
        public bool IsProcessorSpecific => Type >= ElfSectionType.FirstProcessorSpecificSection && Type <= ElfSectionType.LastProcessorSpecificSection;
        public bool IsApplicationSpecific => Type >= ElfSectionType.FirstApplicationSpecificSection && Type <= ElfSectionType.LastApplicationSpecificSection;

        public ElfSectionFlags Flags { get; }
        /// <remarks>
        /// As per the linking rules https://refspecs.linuxfoundation.org/elf/gabi4+/ch4.sheader.html#linking_rules, this returns true if <see cref="ElfSectionFlags.OperatingSystemNonconforming"/> is set.
        /// </remarks>
        public bool HasOperatingSystemSpecificFlags => (Flags & (ElfSectionFlags.OperatingSystemNonconforming | ElfSectionFlags.OperatingSystemSpecificMask)) != ElfSectionFlags.None;
        public bool HasProcessorSpecificFlags => (Flags & ElfSectionFlags.ProcessorSpecificMask) != ElfSectionFlags.None;

        public ulong Address { get; }
        /// <summary>The offset of the section's data within the ELF file.</summary>
        /// <remarks>If <see cref="Type"/> is <see cref="ElfSectionType.NoData"/>, this locates the conceptual placement in the file.</remarks>
        public long OffsetInFile { get; }
        /// <summary>The section's size in bytes.</summary>
        /// <remarks>If <see cref="Type"/> is <see cref="ElfSectionType.NoData"/>, this may not be 0 but does not represent a used range of the file.</remarks>
        public ulong Size { get; }
        /// <summary>This member has special meaning based on <see cref="Type"/>, see https://refspecs.linuxfoundation.org/elf/gabi4+/ch4.sheader.html#sh_link for details.</summary>
        public uint Link { get; }
        /// <summary>This member has special meaning based on <see cref="Type"/>, see https://refspecs.linuxfoundation.org/elf/gabi4+/ch4.sheader.html#sh_link for details.</summary>
        public uint Info { get; }
        /// <summary>The required alignment of the section at runtime. 0/1 indicate no alignment constraints.</summary>
        public ulong Alignment { get; }
        /// <summary>For sections which contain fixed-sized entities, represents the size of said entity in bytes.</summary>
        public ulong EntitySize { get; }

        internal ElfSectionHeader(bool is64Bit, Stream stream)
        {
            ulong ReadNativeWord()
                => is64Bit ? stream.Read<ulong>() : stream.Read<uint>();

            ulong ReadAddress()
                => ReadNativeWord();

            long ReadOffset()
                => checked((long)ReadNativeWord());

            HeaderStart = stream.Position;

            // Elf32_Word Elf64_Word sh_name
            NameIndex = stream.Read<uint>();

            // Elf32_Word Elf64_Word sh_type
            Type = stream.Read<ElfSectionType>();

            // Elf32_Word Elf64_Xword sh_flags
            Flags = (ElfSectionFlags)ReadNativeWord();

            // Elf32_Addr Elf64_Addr sh_addr
            Address = ReadAddress();

            // Elf32_Off Elf64_Off sh_offset
            OffsetInFile = ReadOffset();

            // Elf32_Word Elf64_Xword sh_size
            Size = ReadNativeWord();

            // Elf32_Word Elf64_Word sh_link
            Link = stream.Read<uint>();

            // Elf32_Word Elf64_Word sh_info
            Info = stream.Read<uint>();

            // Elf32_Word Elf64_Xword sh_addralign
            Alignment = ReadNativeWord();

            // Elf32_Word Elf64_Xword sh_entsize
            EntitySize = ReadNativeWord();
        }

        public override string ToString()
            => $"{Type} section @ {OffsetInFile}..{Size}";
    }
}

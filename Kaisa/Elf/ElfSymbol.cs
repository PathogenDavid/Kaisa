﻿using System;
using System.Diagnostics;
using System.IO;

namespace Kaisa.Elf
{
    // https://refspecs.linuxfoundation.org/elf/gabi4+/ch4.symtab.html
    // x64-specific: https://refspecs.linuxfoundation.org/elf/x86_64-abi-0.99.pdf#page=69 (nothing relevant to us.)
    public readonly struct ElfSymbol
    {
        internal const ushort Size32 = sizeof(int) * 3 + sizeof(byte) * 2 + sizeof(ushort);
        internal const ushort Size64 = sizeof(int) + sizeof(byte) * 2 + sizeof(ushort) + sizeof(ulong) * 2;

        /// <summary>The name of this symbol.</summary>
        public string? Name { get; }
        /// <summary>The value of the symbol, exact meaning depends on type of ELF file being parsed.</summary>
        public ulong Value { get; }
        /// <summary>The size of the symbol, 0 if not known.</summary>
        public ulong Size { get; }
        /// <summary>The symbol's binding, which determines the linkage visibility and behavior.</summary>
        public ElfSymbolBinding Binding { get; }
        /// <summary>The symbol's type, which provides a general classification for the associated entity.</summary>
        public ElfSymbolType Type { get; }
        /// <summary>A symbol's visibility defines how that symbol may be accessed once it has become part of an executable or shared object.</summary>
        public ElfSymbolVisibility Visibility { get; }

        private readonly ElfSectionLink DefinedInSectionLink;
        /// <summary>The section which defines this symbol.</summary>
        public ElfSection DefinedIn => DefinedInSectionLink;

        internal ElfSymbol(ElfFile file, Stream stream, ElfStringTableSection? symbolStringTable)
        {
            uint nameIndex;
            ushort definedInSectionIndex;
            byte rawInfo;
            byte rawOther;

            if (file.Is64Bit)
            {
                // Elf64_Word st_name
                nameIndex = stream.Read<uint>();
                // unsigned char st_info
                rawInfo = stream.Read<byte>();
                // unsigned char st_other
                rawOther = stream.Read<byte>();
                // Elf64_Half st_shndx
                definedInSectionIndex = stream.Read<ushort>();
                // Elf64_Addr st_value
                Value = stream.Read<ulong>();
                // Elf64_Xword st_size
                Size = stream.Read<ulong>();
            }
            else
            {
                // Elf32_Word st_name
                nameIndex = stream.Read<uint>();
                // Elf32_Addr st_value
                Value = stream.Read<uint>();
                // Elf32_Word st_size
                Size = stream.Read<uint>();
                // unsigned char st_info
                rawInfo = stream.Read<byte>();
                // unsigned char st_other
                rawOther = stream.Read<byte>();
                // Elf32_Half st_shndx
                definedInSectionIndex = stream.Read<ushort>();
            }

            // Load the name from the symbol string table
            if (nameIndex == 0)
            { Name = null; }
            else
            {
                Debug.Assert(symbolStringTable is not null);
                Name = symbolStringTable.GetString(nameIndex);
            }

            // Link to the definining section
            if (definedInSectionIndex == ElfSection.SHN_XINDEX)
            { throw new NotSupportedException("Kaisa does not support symbols defined indirectly via SHN_XINDEX."); } //TODO

            DefinedInSectionLink = new ElfSectionLink(file, definedInSectionIndex);

            // ELF*_ST_BIND
            Binding = (ElfSymbolBinding)(rawInfo >> 4);
            // ELF*_ST_TYPE
            Type = (ElfSymbolType)(rawInfo & 0xF);
            // ELF*_ST_VISIBILITY
            Visibility = (ElfSymbolVisibility)(rawOther & 0x3);
        }

        public override string ToString()
            => $"{Name ?? "<Unnamed>"} {Visibility} {Binding} {Type} symbol from {DefinedInSectionLink} @ {Value}..{Size}";
    }
}

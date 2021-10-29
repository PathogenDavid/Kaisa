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

        /// <summary>The raw index which describes where this symbol is defined.</summary>
        public ushort RawSectionIndex { get; }

        private readonly ElfSectionLink DefinedInSectionLink;
        /// <summary>The section which defines this symbol.</summary>
        public ElfSection? DefinedIn => DefinedInSectionLink;

        internal ElfSymbol(ElfFile file, Stream stream, int symbolIndex, ElfStringTableSection? symbolStringTable, ElfSymbolTableExtendedIndicesSection? extendedIndices)
        {
            uint nameIndex;
            byte rawInfo;
            byte rawOther;
            long symbolStart = stream.Position;

            if (file.Is64Bit)
            {
                // Elf64_Word st_name
                nameIndex = stream.Read<uint>();
                // unsigned char st_info
                rawInfo = stream.Read<byte>();
                // unsigned char st_other
                rawOther = stream.Read<byte>();
                // Elf64_Half st_shndx
                RawSectionIndex = stream.Read<ushort>();
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
                RawSectionIndex = stream.Read<ushort>();
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
            if (RawSectionIndex == ElfSection.SHN_XINDEX)
            {
                if (extendedIndices is null)
                { throw new MalformedFileException($"Symbol '{Name ?? "<Unnamed>"}' requires an symbol table extended index section to resolve its definining section, but none was found.", symbolStart); }

                uint definedInSectionIndex = extendedIndices.SectionIndices[symbolIndex];

                // Entries in the extended index table will be SHN_UNDEF if they are not used
                if (definedInSectionIndex == ElfSection.SHN_UNDEF)
                { throw new MalformedFileException($"Symbol '{Name ?? "<Unnamed>"}' requires an symbol table extended index section to resolve its definining section, but no corresponding entry was found.", symbolStart); }

                DefinedInSectionLink = ElfSectionLink.MakeIntegerLink(file, definedInSectionIndex);
            }
            else if (RawSectionIndex >= ElfSection.SHN_LORESERVE) // For reserved sections set the link to null. The user will have to use the raw index if they know how to handle it.
            { DefinedInSectionLink = ElfSectionLink.MakeNullLink(file); }
            else
            { DefinedInSectionLink = ElfSectionLink.MakeShortLink(file, RawSectionIndex); }

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

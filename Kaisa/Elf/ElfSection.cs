using System.Diagnostics;

namespace Kaisa.Elf
{
    public abstract class ElfSection
    {
        internal ElfFile File { get; }
        public ElfSectionHeader Header { get; }

        /// <summary>The start of this section's data within the originating stream.</summary>
        public long DataStart => File.FileStartOffset + Header.OffsetInFile;
        /// <summary>The length of this section's data.</summary>
        public ulong DataLength => Header.Size;

        public int Index { get; }
        public string? Name { get; }

        public ElfSectionType Type => Header.Type;
        public bool IsNonStandard
            => Header.IsOperatingSystemSpecific
            || Header.IsProcessorSpecific
            || Header.IsApplicationSpecific
            || Header.HasOperatingSystemSpecificFlags
            || Header.HasProcessorSpecificFlags
            ;

        /// <summary>Whether this section is known to have a special meaning understood by the system. (IE: The <c>.bss</c> or <c>.text</c> sections.)</summary>
        /// <remarks>
        /// A full listing of well-known sections can be found in https://refspecs.linuxfoundation.org/elf/gabi4+/ch4.sheader.html#special_sections
        /// 
        /// The value will be <c>false</c> if the section does not have the expected properties set.
        /// </remarks>
        public bool IsWellKnownSystemSection { get; }

        internal ElfSection(ElfFile file, ElfSectionHeader header, int index)
            : this(file, header, index, LookupName(file, header, index))
        { }

        private static string? LookupName(ElfFile file, ElfSectionHeader header, int index)
        {
            if (header.NameIndex == 0)
            { return null; }
            else if (file.SectionNameTable is null)
            { throw new MalformedFileException($"Section {index} indicates it has a name at {header.NameIndex}, but the file does not have a section name table.", header.HeaderStart); }
            else if (header.NameIndex >= file.SectionNameTable.DataLength)
            { throw new MalformedFileException($"Section {index} indicates it has a name at {header.NameIndex}, but the section name table ends at {file.SectionNameTable.DataLength}.", header.HeaderStart); }
            else
            { return file.SectionNameTable.GetString(header.NameIndex); }
        }

        internal ElfSection(ElfFile file, ElfSectionHeader header, int index, string? name)
        {
            File = file;
            Header = header;
            Index = index;
            Name = name;

            // Check if the section is a well-known system section
            {
                ElfSectionFlags dontCare = ~ElfSectionFlags.None;

                // https://refspecs.linuxfoundation.org/elf/gabi4+/ch4.sheader.html#special_sections
                (bool isWellKnownName, ElfSectionType expectedType, ElfSectionFlags expectedFlags) = Name switch
                {
                    ".bss" => (true, ElfSectionType.NoData, ElfSectionFlags.Allocated | ElfSectionFlags.Writeable),
                    ".comment" => (true, ElfSectionType.ProgramSpecificData, ElfSectionFlags.None),
                    ".data" => (true, ElfSectionType.ProgramSpecificData, ElfSectionFlags.Allocated | ElfSectionFlags.Writeable),
                    ".data1" => (true, ElfSectionType.ProgramSpecificData, ElfSectionFlags.Allocated | ElfSectionFlags.Writeable),
                    ".debug" => (true, ElfSectionType.ProgramSpecificData, ElfSectionFlags.None),
                    ".dynamic" => (true, ElfSectionType.DynamicLinkingInformation, ElfSectionFlags.Allocated | (Header.Flags & ElfSectionFlags.Writeable)),
                    ".dynstr" => (true, ElfSectionType.StringTable, ElfSectionFlags.Allocated),
                    ".dynsym" => (true, ElfSectionType.DynamicSymbolTable, ElfSectionFlags.Allocated),
                    ".fini" => (true, ElfSectionType.ProgramSpecificData, ElfSectionFlags.Allocated | ElfSectionFlags.ExecutableInstructions),
                    ".fini_array" => (true, ElfSectionType.FinalizationFunctions, ElfSectionFlags.Allocated | ElfSectionFlags.Writeable),
                    ".got" => (true, ElfSectionType.ProgramSpecificData, dontCare),
                    ".hash" => (true, ElfSectionType.SymbolHashTable, ElfSectionFlags.Allocated),
                    ".init" => (true, ElfSectionType.ProgramSpecificData, ElfSectionFlags.Allocated | ElfSectionFlags.ExecutableInstructions),
                    ".init_array" => (true, ElfSectionType.InitializationFunctions, ElfSectionFlags.Allocated | ElfSectionFlags.Writeable),
                    ".interp" => (true, ElfSectionType.ProgramSpecificData, Header.Flags & ElfSectionFlags.Allocated),
                    ".line" => (true, ElfSectionType.ProgramSpecificData, ElfSectionFlags.None),
                    ".note" => (true, ElfSectionType.Note, ElfSectionFlags.None),
                    ".plt" => (true, ElfSectionType.ProgramSpecificData, dontCare),
                    ".preinit_array" => (true, ElfSectionType.PreInitializationFunctions, ElfSectionFlags.Allocated | ElfSectionFlags.Writeable),
                    not null when Name.StartsWith(".rel.") => (true, ElfSectionType.RelocationEntries, Header.Flags & ElfSectionFlags.Allocated),
                    not null when Name.StartsWith(".rela.") => (true, ElfSectionType.RelocationEntriesWithExplicitAddends, Header.Flags & ElfSectionFlags.Allocated),
                    ".rodata" => (true, ElfSectionType.ProgramSpecificData, ElfSectionFlags.Allocated),
                    ".rodata1" => (true, ElfSectionType.ProgramSpecificData, ElfSectionFlags.Allocated),
                    ".shstrtab" => (true, ElfSectionType.StringTable, ElfSectionFlags.None),
                    ".strtab" => (true, ElfSectionType.StringTable, Header.Flags & ElfSectionFlags.Allocated),
                    ".symtab" => (true, ElfSectionType.SymbolTable, Header.Flags & ElfSectionFlags.Allocated),
                    ".symtab_shndx" => (true, ElfSectionType.SymbolTableExtendedIndices, Header.Flags & ElfSectionFlags.Allocated),
                    ".tbss" => (true, ElfSectionType.NoData, ElfSectionFlags.Allocated | ElfSectionFlags.Writeable | ElfSectionFlags.ThreadLocalStorage),
                    ".tdata" => (true, ElfSectionType.ProgramSpecificData, ElfSectionFlags.Allocated | ElfSectionFlags.Writeable | ElfSectionFlags.ThreadLocalStorage),
                    ".tdata1" => (true, ElfSectionType.ProgramSpecificData, ElfSectionFlags.Allocated | ElfSectionFlags.Writeable | ElfSectionFlags.ThreadLocalStorage),
                    ".text" => (true, ElfSectionType.ProgramSpecificData, ElfSectionFlags.Allocated | ElfSectionFlags.ExecutableInstructions),
                    _ => (false, default(ElfSectionType), default(ElfSectionFlags))
                };

                // x64-specific special sections https://refspecs.linuxfoundation.org/elf/x86_64-abi-0.99.pdf#page=63
                if (File.Header.Machine == ElfMachine.X86_64)
                {
                    switch (Name)
                    {
                        case ".got":
                            Debug.Assert(isWellKnownName && expectedType == ElfSectionType.ProgramSpecificData);
                            expectedFlags = ElfSectionFlags.Allocated | ElfSectionFlags.Writeable;
                            break;
                        case ".plt":
                            Debug.Assert(isWellKnownName && expectedType == ElfSectionType.ProgramSpecificData);
                            expectedFlags = ElfSectionFlags.Allocated | ElfSectionFlags.ExecutableInstructions;
                            break;
                        case ".eh_frame":
                            isWellKnownName = true;
                            expectedType = ElfSectionType.X64UnwindFunctionTable;
                            expectedFlags = ElfSectionFlags.Allocated;

                            // In practice this can be SHT_PROGBITS too even though the specs say otherwise
                            if (Type == ElfSectionType.ProgramSpecificData)
                            { expectedType = ElfSectionType.ProgramSpecificData; }
                            break;
                        // Large code model sections
                        case ".lbss":
                        case ".ldata":
                        case ".ldata1":
                        case ".lgot":
                        case ".lplt":
                        case ".lrodata":
                        case ".lrodata1":
                        case ".ltext":
                            isWellKnownName = true;
                            expectedType = Name is ".lbss" ? ElfSectionType.NoData : ElfSectionType.ProgramSpecificData;
                            expectedFlags = ElfSectionFlags.Allocated | ElfSectionFlags.X64Large;

                            switch (Name)
                            {
                                case ".lbss":
                                case ".ldata":
                                case ".ldata1":
                                case ".lgot":
                                    expectedFlags |= ElfSectionFlags.Writeable;
                                    break;
                                case ".lplt":
                                case ".ltext":
                                    expectedFlags |= ElfSectionFlags.ExecutableInstructions;
                                    break;
                            }
                            break;
                    }
                }

                if (isWellKnownName)
                {
                    IsWellKnownSystemSection = true;

                    // Validate the type
                    if (Header.Type != expectedType)
                    {
                        Debug.Fail("Found section with a well-known name but with an incorrect type!");
                        IsWellKnownSystemSection = false;
                    }

                    // Validate the flags
                    // The spec doesn't actually say whether the flags it lists should be the only flags or if those flags should simply be present.
                    // I originally interpreted it as the former since it also lists certain flags which are optional, but in practice it seems like the latter is true so that's what we check.
                    if (expectedFlags != dontCare && (Header.Flags & expectedFlags) != expectedFlags)
                    {
                        Debug.Fail("Found section with a well-known name that is missing required flags.");
                        IsWellKnownSystemSection = false;
                    }
                }
            }
        }

        protected virtual string SectionTypeString
            => GetType().Name;

        public override string ToString()
            => $"[{Index}] {SectionTypeString}: '{Name ?? "<Unnamed>"}' @ 0x{DataStart:X}..0x{(ulong)DataStart + DataLength} ({DataLength} bytes)";

        // Special index values, Kaisa generally attempts to hide these so they keep their unfriendly spec names.
        // Kaisa generally just uses 0 instead of SHN_UNDEF.
        internal const ushort SHN_UNDEF = 0;
        internal const ushort SHN_LORESERVE = 0xff00;
        internal const ushort SHN_LOPROC = 0xff00;
        internal const ushort SHN_HIPROC = 0xff1f;
        internal const ushort SHN_LOOS = 0xff20;
        internal const ushort SHN_HIOS = 0xff3f;
        internal const ushort SHN_ABS = 0xfff1;
        internal const ushort SHN_COMMON = 0xfff2;
        internal const ushort SHN_XINDEX = 0xffff;
        internal const ushort SHN_HIRESERVE = 0xffff;
    }
}

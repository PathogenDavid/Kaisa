namespace Kaisa.Elf
{
    // https://refspecs.linuxfoundation.org/elf/gabi4+/ch4.sheader.html#sh_type
    public enum ElfSectionType : uint
    {
        /// <summary>This value marks the section header as inactive; it does not have an associated section. Other members of the section header have undefined values.</summary>
        Null = 0,
        /// <summary>The section holds information defined by the program, whose format and meaning are determined solely by the program.</summary>
        ProgramSpecificData = 1,
        /// <summary>The section holds a complete symbol table, typically for static linking (but may be used for dynamic linking.)</summary>
        /// <remarks>An ELF file may only have one symbol table.</remarks>
        SymbolTable = 2,
        /// <summary>The section holds a string table.</summary>
        StringTable = 3,
        /// <summary>The section holds relocation entries with explicit addends.</summary>
        RelocationEntriesWithExplicitAddends = 4,
        /// <summary>The section holds a symbol hash table.</summary>
        /// <remarks>An ELF file may only have one symbol hash table.</remarks>
        SymbolHashTable = 5,
        /// <summary>The section holds information for dynamic linking.</summary>
        /// <remarks>An ELF file may only have one section of this type.</remarks>
        DynamicLinkingInformation = 6,
        /// <summary>The section holds information that marks the file in some way.</summary>
        Note = 7,
        /// <summary>A section of this type occupies no space in the file but otherwise resembles <see cref="ProgramSpecificData"/>.</summary>
        /// <remarks>Although this section contains no bytes, the sh_offset member contains the conceptual file offset.</remarks>
        NoData = 8,
        /// <summary>The section holds relocation entries without explicit addends.</summary>
        RelocationEntries = 9,
        /// <summary>This section type is reserved but has unspecified semantics.</summary>
        SHLIB = 10,
        /// <summary>The section holdes a minimal set of dynamic linking symbols.</summary>
        /// <remarks>An ELF file may only have one dynamic symbol table.</remarks>
        DynamicSymbolTable = 11,
        /// <summary>This section contains an array of pointers to initialization functions.</summary>
        InitializationFunctions = 14,
        /// <summary>This section contains an array of pointers to termination functions.</summary>
        FinalizationFunctions = 15,
        /// <summary>This section contains an array of pointers to functions that are invoked before all other initialization functions.</summary>
        PreInitializationFunctions = 16,
        /// <summary>This section defines a section group.</summary>
        /// <remarks>Only appears in ELF files of type <see cref="ElfType.RelocatableFile"/>.</remarks>
        Group = 17,
        /// <summary>
        /// This section is associated with a section of type SHT_SYMTAB and is required if any of the section header indexes referenced by
        /// that symbol table contain the escape value SHN_XINDEX.
        /// </summary>
        SymbolTableExtendedIndices = 18,

        FirstOperatingSystemSpecificSection = 0x60000000,
        LastOperatingSystemSpecificSection = 0x6fffffff,
        FirstProcessorSpecificSection = 0x70000000,
        LastProcessorSpecificSection = 0x7fffffff,
        FirstApplicationSpecificSection = 0x80000000,
        LastApplicationSpecificSection = 0xffffffff,

        // ==========================================================================================
        // x64-specific types
        // ==========================================================================================
        /// <summary>x64-specific: This section contains unwind function table entries for stack unwinding.</summary>
        X64UnwindFunctionTable = 0x70000001,
    }
}

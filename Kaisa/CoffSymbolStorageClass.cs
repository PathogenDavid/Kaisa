namespace Kaisa
{
    // https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#storage-class
    public enum CoffSymbolStorageClass : byte
    {
        /// <summary>A special symbol that represents the end of function, for debugging purposes.</summary>
        EndOfFunction = 0xFF,
        /// <summary>No assigned storage class.</summary>
        Null = 0,
        /// <summary>The automatic (stack) variable. The Value field specifies the stack frame offset.</summary>
        Automatix = 1,
        /// <summary>A value that Microsoft tools use for external symbols. The Value field indicates the size if the section number is IMAGE_SYM_UNDEFINED (0). If the section number is not zero, then the Value field specifies the offset within the section.</summary>
        External = 2,
        /// <summary>The offset of the symbol within the section. If the Value field is zero, then the symbol represents a section name.</summary>
        Static = 3,
        /// <summary>A register variable. The Value field specifies the register number.</summary>
        Register = 4,
        /// <summary>A symbol that is defined externally.</summary>
        ExternalDefinition = 5,
        /// <summary>A code label that is defined within the module. The Value field specifies the offset of the symbol within the section.</summary>
        Label = 6,
        /// <summary>A reference to a code label that is not defined.</summary>
        UndefinedLabel = 7,
        /// <summary>The structure member. The Value field specifies the n th member.</summary>
        MemberOfStruct = 8,
        /// <summary>A formal argument (parameter) of a function. The Value field specifies the n th argument.</summary>
        Argument = 9,
        /// <summary>The structure tag-name entry.</summary>
        StructTag = 10,
        /// <summary>A union member. The Value field specifies the n th member.</summary>
        MemberOfUnion = 11,
        /// <summary>The Union tag-name entry.</summary>
        UnionTag = 12,
        /// <summary>A Typedef entry.</summary>
        TypeDefinition = 13,
        /// <summary>A static data declaration.</summary>
        UndefinedStatic = 14,
        /// <summary>An enumerated type tagname entry.</summary>
        EnumTag = 15,
        /// <summary>A member of an enumeration. The Value field specifies the n th member.</summary>
        MemberOfEnum = 16,
        /// <summary>A register parameter.</summary>
        RegisterParameter = 17,
        /// <summary>A bit-field reference. The Value field specifies the n th bit in the bit field.</summary>
        BitField = 18,
        /// <summary>A .bb (beginning of block) or .eb (end of block) record. The Value field is the relocatable address of the code location.</summary>
        Block = 100,
        /// <summary>A value that Microsoft tools use for symbol records that define the extent of a function: begin function (.bf ), end function ( .ef ), and lines in function ( .lf ). For .lf records, the Value field gives the number of source lines in the function. For .ef records, the Value field gives the size of the function code.</summary>
        Function = 101,
        /// <summary>An end-of-structure entry.</summary>
        EndOfStruct = 102,
        /// <summary>A value that Microsoft tools, as well as traditional COFF format, use for the source-file symbol record. The symbol is followed by auxiliary records that name the file.</summary>
        File = 103,
        /// <summary>A definition of a section (Microsoft tools use STATIC storage class instead).</summary>
        Section = 104,
        /// <summary>A weak external. For more information, see Auxiliary Format 3: Weak Externals.</summary>
        WeakExternal = 105,
        /// <summary>A CLR token symbol. The name is an ASCII string that consists of the hexadecimal value of the token. For more information, see CLR Token Definition (Object Only).</summary>
        ClrToken = 107,
    }
}

namespace Kaisa
{
    // https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#type-representation
    public enum CoffSymbolBaseType : byte
    {
        /// <summary>No type information or unknown base type. Microsoft tools use this setting</summary>
        Null = 0,
        /// <summary>No valid type; used with void pointers and functions</summary>
        Void = 1,
        /// <summary>A character (signed byte)</summary>
        Char = 2,
        /// <summary>A 2-byte signed integer</summary>
        Short = 3,
        /// <summary>A natural integer type (normally 4 bytes in Windows)</summary>
        Int = 4,
        /// <summary>A 4-byte signed integer</summary>
        Long = 5,
        /// <summary>A 4-byte floating-point number</summary>
        Float = 6,
        /// <summary>An 8-byte floating-point number</summary>
        Double = 7,
        /// <summary>A structure</summary>
        Struct = 8,
        /// <summary>A union</summary>
        Union = 9,
        /// <summary>An enumerated type</summary>
        Enum = 10,
        /// <summary>A member of enumeration (a specific value)</summary>
        MemberOfEnumeration = 11,
        /// <summary>A byte; unsigned 1-byte integer</summary>
        Byte = 12,
        /// <summary>A word; unsigned 2-byte integer</summary>
        Word = 13,
        /// <summary>An unsigned integer of natural size (normally, 4 bytes)</summary>
        UInt = 14,
        /// <summary>An unsigned 4-byte integer</summary>
        DWord = 15,
    }
}

using System;

namespace Kaisa.Elf
{
    // https://refspecs.linuxfoundation.org/elf/gabi4+/ch4.sheader.html#sh_flags
    [Flags]
    public enum ElfSectionFlags : uint
    {
        None = 0,
        /// <summary>The section contains data that should be writable during process execution.</summary>
        Writeable = 0x1,
        /// <summary>The section occupies memory during process execution. Some control sections do not reside in the memory image of an object file; this attribute is off for those sections.</summary>
        Allocated = 0x2,
        /// <summary>The section contains executable machine instructions.</summary>
        ExecutableInstructions = 0x4,
        /// <summary>The data in the section may be merged to eliminate duplication.</summary>
        MayMerge = 0x10,
        /// <summary>The data elements in the section consist of null-terminated character strings.</summary>
        Strings = 0x20,
        /// <summary>The <c>sh_info</c> field of this section header holds a section header table index.</summary>
        HasInfoLink = 0x40,
        /// <summary>This flag adds special ordering requirements for link editors.</summary>
        LinkOrderIsSignificant = 0x80,
        /// <summary>This section requires special OS-specific processing (beyond the standard linking rules) to avoid incorrect behavior.</summary>
        OperatingSystemNonconforming = 0x100,
        /// <summary>This section is a member (perhaps the only one) of a section group.</summary>
        GroupMember = 0x200,
        /// <summary>This section holds Thread-Local Storage, meaning that each separate execution flow has its own distinct instance of this data.</summary>
        ThreadLocalStorage = 0x400,
        /// <summary>All bits included in this mask are reserved for operating system-specific semantics.</summary>
        OperatingSystemSpecificMask = 0x0ff00000,
        /// <summary>All bits included in this mask are reserved for processor-specific semantics. If meanings are specified, the processor supplement explains them.</summary>
        ProcessorSpecificMask = 0xf0000000,

        // ==========================================================================================
        // x64-Specific Flags
        // ==========================================================================================
        /// <summary>x64-specific: Indicates this section exceeds 2 GB.</summary>
        /// <remarks>See https://refspecs.linuxfoundation.org/elf/x86_64-abi-0.99.pdf#page=62 for details.</remarks>
        X64Large = 0x10000000,
    }
}

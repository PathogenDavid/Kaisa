using System;

namespace Kaisa
{
    [Flags]
    public enum SectionFlags : uint
    {
        /// <summary>The section should not be padded to the next boundary. This flag is obsolete and is replaced by IMAGE_SCN_ALIGN_1BYTES. This is valid only for object files.</summary>
        TYPE_NO_PAD = 0x00000008,
        /// <summary>The section contains executable code.</summary>
        CNT_CODE = 0x00000020,
        /// <summary>The section contains initialized data.</summary>
        CNT_INITIALIZED_DATA = 0x00000040,
        /// <summary>The section contains uninitialized data.</summary>
        CNT_UNINITIALIZED_DATA = 0x00000080,
        /// <summary>Reserved for future use.</summary>
        LNK_OTHER = 0x00000100,
        /// <summary>The section contains comments or other information. The .drectve section has this type. This is valid for object files only.</summary>
        LNK_INFO = 0x00000200,
        /// <summary>The section will not become part of the image. This is valid only for object files.</summary>
        LNK_REMOVE = 0x00000800,
        /// <summary>The section contains COMDAT data. For more information, see COMDAT Sections (Object Only). This is valid only for object files.</summary>
        LNK_COMDAT = 0x00001000,
        /// <summary>The section contains data referenced through the global pointer (GP).</summary>
        GPREL = 0x00008000,
        /// <summary>Reserved for future use.</summary>
        MEM_PURGEABLE = 0x00020000,
        /// <summary>Reserved for future use.</summary>
        MEM_16BIT = 0x00020000,
        /// <summary>Reserved for future use.</summary>
        MEM_LOCKED = 0x00040000,
        /// <summary>Reserved for future use.</summary>
        MEM_PRELOAD = 0x00080000,
        /// <summary>Align data on a 1-byte boundary. Valid only for object files.</summary>
        ALIGN_1BYTES = 0x00100000,
        /// <summary>Align data on a 2-byte boundary. Valid only for object files.</summary>
        ALIGN_2BYTES = 0x00200000,
        /// <summary>Align data on a 4-byte boundary. Valid only for object files.</summary>
        ALIGN_4BYTES = 0x00300000,
        /// <summary>Align data on an 8-byte boundary. Valid only for object files.</summary>
        ALIGN_8BYTES = 0x00400000,
        /// <summary>Align data on a 16-byte boundary. Valid only for object files.</summary>
        ALIGN_16BYTES = 0x00500000,
        /// <summary>Align data on a 32-byte boundary. Valid only for object files.</summary>
        ALIGN_32BYTES = 0x00600000,
        /// <summary>Align data on a 64-byte boundary. Valid only for object files.</summary>
        ALIGN_64BYTES = 0x00700000,
        /// <summary>Align data on a 128-byte boundary. Valid only for object files.</summary>
        ALIGN_128BYTES = 0x00800000,
        /// <summary>Align data on a 256-byte boundary. Valid only for object files.</summary>
        ALIGN_256BYTES = 0x00900000,
        /// <summary>Align data on a 512-byte boundary. Valid only for object files.</summary>
        ALIGN_512BYTES = 0x00A00000,
        /// <summary>Align data on a 1024-byte boundary. Valid only for object files.</summary>
        ALIGN_1024BYTES = 0x00B00000,
        /// <summary>Align data on a 2048-byte boundary. Valid only for object files.</summary>
        ALIGN_2048BYTES = 0x00C00000,
        /// <summary>Align data on a 4096-byte boundary. Valid only for object files.</summary>
        ALIGN_4096BYTES = 0x00D00000,
        /// <summary>Align data on an 8192-byte boundary. Valid only for object files.</summary>
        ALIGN_8192BYTES = 0x00E00000,
        /// <summary>The section contains extended relocations.</summary>
        LNK_NRELOC_OVFL = 0x01000000,
        /// <summary>The section can be discarded as needed.</summary>
        MEM_DISCARDABLE = 0x02000000,
        /// <summary>The section cannot be cached.</summary>
        MEM_NOT_CACHED = 0x04000000,
        /// <summary>The section is not pageable.</summary>
        MEM_NOT_PAGED = 0x08000000,
        /// <summary>The section can be shared in memory.</summary>
        MEM_SHARED = 0x10000000,
        /// <summary>The section can be executed as code.</summary>
        MEM_EXECUTE = 0x20000000,
        /// <summary>The section can be read.</summary>
        MEM_READ = 0x40000000,
        /// <summary>The section can be written to.</summary>
        MEM_WRITE = 0x80000000,
    }
}

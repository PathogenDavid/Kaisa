﻿using System;

namespace Kaisa
{
    [Flags]
    public enum ImageFileCharacteristics : ushort
    {
        None = 0,
        /// <summary>Image only, Windows CE, and Microsoft Windows NT and later. This indicates that the file does not contain base relocations and must therefore be loaded at its preferred base address. If the base address is not available, the loader reports an error. The default behavior of the linker is to strip base relocations from executable (EXE) files.</summary>
        RELOCS_STRIPPED = 0x0001,
        /// <summary>Image only. This indicates that the image file is valid and can be run. If this flag is not set, it indicates a linker error.</summary>
        EXECUTABLE_IMAGE = 0x0002,
        /// <summary>COFF line numbers have been removed. This flag is deprecated and should be zero.</summary>
        LINE_NUMS_STRIPPED = 0x0004,
        /// <summary>COFF symbol table entries for local symbols have been removed. This flag is deprecated and should be zero.</summary>
        LOCAL_SYMS_STRIPPED = 0x0008,
        /// <summary>Obsolete. Aggressively trim working set. This flag is deprecated for Windows 2000 and later and must be zero.</summary>
        AGGRESSIVE_WS_TRIM = 0x0010,
        /// <summary>Little endian: the least significant bit (LSB) precedes the most significant bit (MSB) in memory. This flag is deprecated and should be zero.</summary>
        BYTES_REVERSED_LO = 0x0080,
        /// <summary>Machine is based on a 32-bit-word architecture.</summary>
        _32BIT_MACHINE = 0x0100,
        /// <summary>Debugging information is removed from the image file.</summary>
        DEBUG_STRIPPED = 0x0200,
        /// <summary>If the image is on network media, fully load it and copy it to the swap file.</summary>
        NET_RUN_FROM_SWAP = 0x0800,
        /// <summary>The image file is a system file, not a user program.</summary>
        SYSTEM = 0x1000,
        /// <summary>The image file is a dynamic-link library (DLL). Such files are considered executable files for almost all purposes, although they cannot be directly run.</summary>
        DLL = 0x2000,
        /// <summary>The file should be run only on a uniprocessor machine.</summary>
        UP_SYSTEM_ONLY = 0x4000,
        /// <summary>Big endian: the MSB precedes the LSB in memory. This flag is deprecated and should be zero.</summary>
        BYTES_REVERSED_HI = 0x8000,
    }
}

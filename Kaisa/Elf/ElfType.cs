namespace Kaisa.Elf
{
    // e_type from https://refspecs.linuxfoundation.org/elf/gabi4+/ch4.eheader.html
    public enum ElfType : ushort
    {
        /// <summary>No file type</summary>
        None = 0,
        /// <summary>Relocatable file</summary>
        RelocatableFile = 1,
        /// <summary>Executable file</summary>
        ExecutableFile = 2,
        /// <summary>Shared object file</summary>
        SharedObjectFile = 3,
        /// <summary>Core file</summary>
        CoreFile = 4,

        /// <summary>Operating system-specific</summary>
        FirstOperatingSystemSpecific = 0xfe00,
        /// <summary>Operating system-specific</summary>
        LastOperatingSystemSpecific = 0xfeff,

        // The processor-specific ranges are not used by x64
        // https://refspecs.linuxfoundation.org/elf/x86_64-abi-0.99.pdf#page=61
        /// <summary>Processor-specific</summary>
        FirstProcessorSpecific = 0xff00,
        /// <summary>Processor-specific</summary>
        LastProcessorSpecific = 0xffff,
    }
}

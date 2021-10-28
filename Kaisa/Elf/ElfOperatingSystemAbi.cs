namespace Kaisa.Elf
{
    // EI_OSABI - https://refspecs.linuxfoundation.org/elf/gabi4+/ch4.eheader.html
    public enum ElfOperatingSystemAbi : byte
    {
        /// <summary>No extensions or unspecified</summary>
        None = 0,
        /// <summary>Hewlett-Packard HP-UX</summary>
        HPUX = 1,
        /// <summary>NetBSD</summary>
        NetBSD = 2,
        /// <summary>Linux</summary>
        Linux = 3,
        /// <summary>Sun Solaris</summary>
        Solaris = 6,
        /// <summary>AIX</summary>
        AIX = 7,
        /// <summary>IRIX</summary>
        IRIX = 8,
        /// <summary>FreeBSD</summary>
        FreeBSD = 9,
        /// <summary>Compaq TRU64 UNIX</summary>
        TRU64 = 10,
        /// <summary>Novell Modesto</summary>
        Modesto = 11,
        /// <summary>Open BSD</summary>
        OpenBSD = 12,
        /// <summary>Open VMS</summary>
        OpenVMS = 13,
        /// <summary>Hewlett-Packard Non-Stop Kernel</summary>
        NonStopKernel = 14,

        /// <summary>The start of the architecture-specific value range</summary>
        FirstArchitectureSpecific = 64,
    }
}

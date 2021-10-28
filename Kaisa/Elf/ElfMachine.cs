namespace Kaisa.Elf
{
    public enum ElfMachine : ushort
    {
        /// <summary>No machine</summary>
        None = 0,
        /// <summary>AT&T WE 32100</summary>
        M32 = 1,
        /// <summary>SPARC</summary>
        SPARC = 2,
        /// <summary>Intel 80386</summary>
        Intel80386 = 3,
        /// <summary>Motorola 68000</summary>
        Intel68000 = 4,
        /// <summary>Motorola 88000</summary>
        Intel88000 = 5,
        /// <summary>Intel 80860</summary>
        Intel80860 = 7,
        /// <summary>MIPS I Architecture</summary>
        MIPS = 8,
        /// <summary>IBM System/370 Processor</summary>
        S370 = 9,
        /// <summary>MIPS RS3000 Little-endian</summary>
        MIPS_RS3_LE = 10,
        /// <summary>Hewlett-Packard PA-RISC</summary>
        PARISC = 15,
        /// <summary>Fujitsu VPP500</summary>
        VPP500 = 17,
        /// <summary>Enhanced instruction set SPARC</summary>
        SPARC32PLUS = 18,
        /// <summary>Intel 80960</summary>
        Intel80960 = 19,
        /// <summary>PowerPC</summary>
        PPC = 20,
        /// <summary>64-bit PowerPC</summary>
        PPC64 = 21,
        /// <summary>IBM System/390 Processor</summary>
        S390 = 22,
        /// <summary>NEC V800</summary>
        V800 = 36,
        /// <summary>Fujitsu FR20</summary>
        FR20 = 37,
        /// <summary>TRW RH-32</summary>
        RH32 = 38,
        /// <summary>Motorola RCE</summary>
        RCE = 39,
        /// <summary>Advanced RISC Machines ARM</summary>
        ARM = 40,
        /// <summary>Digital Alpha</summary>
        ALPHA = 41,
        /// <summary>Hitachi SH</summary>
        SH = 42,
        /// <summary>SPARC Version 9</summary>
        SPARCV9 = 43,
        /// <summary>Siemens TriCore embedded processor</summary>
        TRICORE = 44,
        /// <summary>Argonaut RISC Core, Argonaut Technologies Inc.</summary>
        ARC = 45,
        /// <summary>Hitachi H8/300</summary>
        H8_300 = 46,
        /// <summary>Hitachi H8/300H</summary>
        H8_300H = 47,
        /// <summary>Hitachi H8S</summary>
        H8S = 48,
        /// <summary>Hitachi H8/500</summary>
        H8_500 = 49,
        /// <summary>Intel IA-64 processor architecture</summary>
        IA_64 = 50,
        /// <summary>Stanford MIPS-X</summary>
        MIPS_X = 51,
        /// <summary>Motorola ColdFire</summary>
        COLDFIRE = 52,
        /// <summary>Motorola M68HC12</summary>
        Motorola68HC12 = 53,
        /// <summary>Fujitsu MMA Multimedia Accelerator</summary>
        MMA = 54,
        /// <summary>Siemens PCP</summary>
        PCP = 55,
        /// <summary>Sony nCPU embedded RISC processor</summary>
        NCPU = 56,
        /// <summary>Denso NDR1 microprocessor</summary>
        NDR1 = 57,
        /// <summary>Motorola Star*Core processor</summary>
        STARCORE = 58,
        /// <summary>Toyota ME16 processor</summary>
        ME16 = 59,
        /// <summary>STMicroelectronics ST100 processor</summary>
        ST100 = 60,
        /// <summary>Advanced Logic Corp. TinyJ embedded processor family</summary>
        TINYJ = 61,
        /// <summary>AMD x86-64 architecture</summary>
        X86_64 = 62,
        /// <summary>Sony DSP Processor</summary>
        PDSP = 63,
        /// <summary>Digital Equipment Corp. PDP-10</summary>
        PDP10 = 64,
        /// <summary>Digital Equipment Corp. PDP-11</summary>
        PDP11 = 65,
        /// <summary>Siemens FX66 microcontroller</summary>
        FX66 = 66,
        /// <summary>STMicroelectronics ST9+ 8/16 bit microcontroller</summary>
        ST9PLUS = 67,
        /// <summary>STMicroelectronics ST7 8-bit microcontroller</summary>
        ST7 = 68,
        /// <summary>Motorola MC68HC16 Microcontroller</summary>
        Motorola68HC16 = 69,
        /// <summary>Motorola MC68HC11 Microcontroller</summary>
        Motorola68HC11 = 70,
        /// <summary>Motorola MC68HC08 Microcontroller</summary>
        Motorola68HC08 = 71,
        /// <summary>Motorola MC68HC05 Microcontroller</summary>
        Motorola68HC05 = 72,
        /// <summary>Silicon Graphics SVx</summary>
        SVX = 73,
        /// <summary>STMicroelectronics ST19 8-bit microcontroller</summary>
        ST19 = 74,
        /// <summary>Digital VAX</summary>
        VAX = 75,
        /// <summary>Axis Communications 32-bit embedded processor</summary>
        CRIS = 76,
        /// <summary>Infineon Technologies 32-bit embedded processor</summary>
        JAVELIN = 77,
        /// <summary>Element 14 64-bit DSP Processor</summary>
        FIREPATH = 78,
        /// <summary>LSI Logic 16-bit DSP Processor</summary>
        ZSP = 79,
        /// <summary>Donald Knuth's educational 64-bit processor</summary>
        MMIX = 80,
        /// <summary>Harvard University machine-independent object files</summary>
        HUANY = 81,
        /// <summary>SiTera Prism</summary>
        PRISM = 82,
        /// <summary>Atmel AVR 8-bit microcontroller</summary>
        AVR = 83,
        /// <summary>Fujitsu FR30</summary>
        FR30 = 84,
        /// <summary>Mitsubishi D10V</summary>
        D10V = 85,
        /// <summary>Mitsubishi D30V</summary>
        D30V = 86,
        /// <summary>NEC v850</summary>
        V850 = 87,
        /// <summary>Mitsubishi M32R</summary>
        M32R = 88,
        /// <summary>Matsushita MN10300</summary>
        MN10300 = 89,
        /// <summary>Matsushita MN10200</summary>
        MN10200 = 90,
        /// <summary>picoJava</summary>
        PJ = 91,
        /// <summary>OpenRISC 32-bit embedded processor</summary>
        OPENRISC = 92,
        /// <summary>ARC Cores Tangent-A5</summary>
        ARC_A5 = 93,
        /// <summary>Tensilica Xtensa Architecture</summary>
        XTENSA = 94,
        /// <summary>Alphamosaic VideoCore processor</summary>
        VIDEOCORE = 95,
        /// <summary>Thompson Multimedia General Purpose Processor</summary>
        TMM_GPP = 96,
        /// <summary>National Semiconductor 32000 series</summary>
        NS32K = 97,
        /// <summary>Tenor Network TPC processor</summary>
        TPC = 98,
        /// <summary>Trebia SNP 1000 processor</summary>
        SNP1K = 99,
        /// <summary>STMicroelectronics (www.st.com) ST200 microcontroller</summary>
        ST200 = 100,
    }
}

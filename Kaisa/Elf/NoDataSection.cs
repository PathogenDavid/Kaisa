namespace Kaisa.Elf
{
    /// <summary>Represents an ELF section that has no data. (That is, its <see cref="ElfSectionHeader.Type"/> is <see cref="ElfSectionType.NoData"/>.)</summary>
    public sealed class NoDataSection : ElfSection
    {
        internal NoDataSection(ElfFile file, ElfSectionHeader header, int index)
            : base(file, header, index)
        { }
    }
}

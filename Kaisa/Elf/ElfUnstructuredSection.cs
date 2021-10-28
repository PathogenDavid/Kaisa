namespace Kaisa.Elf
{
    /// <summary>Represents an ELF section which either has no structure by definition or its structure is not parsed by Kaisa.</summary>
    /// <remarks>
    /// The data for this member can be read by seeking the archive's stream's position to <see cref="ElfSection.DataStart"/>
    /// and reading up to <see cref="ElfSection.DataLength"/> bytes.
    /// </remarks>
    public sealed class ElfUnstructuredSection : ElfSection
    {
        internal ElfUnstructuredSection(ElfFile file, ElfSectionHeader header, int index)
            : base(file, header, index)
        { }
    }
}

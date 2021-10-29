using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;

namespace Kaisa.Elf
{
    // https://refspecs.linuxfoundation.org/elf/gabi4+/contents.html
    public sealed class ElfFile : IEnumerable<ElfSection>
    {
        public long FileStartOffset { get; }
        internal bool Is64Bit => Header.Is64Bit;
        public ElfHeader Header { get; }
        internal ElfStringTableSection? SectionNameTable { get; }

        public ImmutableArray<ElfSection> Sections { get; }

        public ElfSymbolTableSection? SymbolTable { get; }
        public ElfSymbolTableSection? DynamicSymbolTable { get; }

        public ElfFile(Stream stream)
            : this(stream.Read<ushort>(), stream.Read<ushort>(), stream)
        { }

        internal ElfFile(ushort sig1, ushort sig2, Stream stream)
        {
            if (!stream.CanSeek)
            { throw new NotSupportedException("Cannot read ELF files from streams which are not seekable."); }

            // Parse the ELF header
            FileStartOffset = stream.Position - (sizeof(ushort) * 2);

            if (!ElfIdentity.IsElfFile(sig1, sig2))
            { throw new MalformedFileException("The specified stream does not appear to represent a valid ELF file, the signature is invalid.", FileStartOffset); }

            ElfIdentity identity = new(sig1, sig2, stream);
            Header = new ElfHeader(identity, stream);

            // Parse the section table
            // https://refspecs.linuxfoundation.org/elf/gabi4+/ch4.sheader.html
            // e_shoff -- "If the file has no section header table, this member holds zero."
            if (Header.SectionHeaderTableFileOffset == 0)
            { Sections = ImmutableArray<ElfSection>.Empty; }
            else
            {
                long sectionTableOffset = FileStartOffset + Header.SectionHeaderTableFileOffset;
                stream.Position = sectionTableOffset;

                // Read 0th section header (it's always present and has special meaning)
                // https://refspecs.linuxfoundation.org/elf/gabi4+/ch4.sheader.html (see Figure 4-10)
                ElfSectionHeader section0 = new(Is64Bit, stream);
                Debug.Assert(section0.NameIndex == 0, "Section 0 should not have a name.");
                Debug.Assert(section0.Type == ElfSectionType.Null, "Section 0 should not have a type.");
                Debug.Assert(section0.Flags == ElfSectionFlags.None, "Section 0 should not have any flags.");
                Debug.Assert(section0.Address == 0, "Section 0 should not have an address.");
                Debug.Assert(section0.OffsetInFile == 0, "Section 0 should not have a file offset.");
                Debug.Assert(section0.Info == 0, "Section 0 should not have any auxillary information.");
                Debug.Assert(section0.Alignment == 0, "Section 0 should not have any alignment.");
                Debug.Assert(section0.EntitySize == 0, "Section 0 should not have an entity size.");

                int sectionCount;
                int sectionNameTableIndex;
                {
                    ulong actualSectionCount = section0.Size;
                    uint actualSectionNameTableIndex = section0.Link;

                    if (actualSectionCount > int.MaxValue)
                    { throw new NotSupportedException($"Cannot parse ELF files with {actualSectionCount} sections."); }

                    if (Header.SectionHeaderTableLength == 0)
                    { sectionCount = checked((int)actualSectionCount); }
                    else
                    {
                        if (actualSectionCount != 0)
                        { throw new MalformedFileException("The ELF file specifies a section count in both the header and the 0th section.", sectionTableOffset); }

                        sectionCount = Header.SectionHeaderTableLength;
                    }

                    if (Header.StringTableSectionIndex == ushort.MaxValue)
                    {
                        // The section string table is specified by the 0th section
                        if (actualSectionNameTableIndex == 0)
                        { throw new MalformedFileException("The ELF header indicates the section name table is provided by the 0th section, but it wasn't.", sectionTableOffset); }
                        else if (actualSectionNameTableIndex > int.MaxValue)
                        { throw new NotSupportedException("The section name table index is too high to be parsed by Kaisa."); }

                        sectionNameTableIndex = checked((int)actualSectionNameTableIndex);
                    }
                    else
                    { sectionNameTableIndex = Header.StringTableSectionIndex; }
                }

                // Read all section headers
                ImmutableArray<ElfSectionHeader> sectionHeaders;
                {
                    ImmutableArray<ElfSectionHeader>.Builder sectionHeadersBuilder = ImmutableArray.CreateBuilder<ElfSectionHeader>(sectionCount);
                    sectionHeadersBuilder.Add(section0);

                    for (int i = 1; i < sectionCount; i++)
                    { sectionHeadersBuilder.Add(new ElfSectionHeader(Is64Bit, stream)); }

                    sectionHeaders = sectionHeadersBuilder.MoveToImmutable();
                }

                // Read the section name string table if we have one
                if (sectionNameTableIndex == 0)
                { SectionNameTable = null; }
                else
                {
                    if (sectionNameTableIndex >= sectionCount)
                    { throw new MalformedFileException($"The section name table was specified at index {sectionNameTableIndex} but the file only contains {sectionCount} sections!", FileStartOffset); }

                    SectionNameTable = ElfStringTableSection.CreateSectionNameTable(this, sectionHeaders[sectionNameTableIndex], stream, sectionNameTableIndex);
                }

                // Read all sections
                ImmutableArray<ElfSection>.Builder sectionsBuilder = ImmutableArray.CreateBuilder<ElfSection>(sectionHeaders.Length);
                sectionsBuilder.Count = sectionHeaders.Length;

                if (SectionNameTable is not null)
                { sectionsBuilder[SectionNameTable.Index] = SectionNameTable; }

                string GetSectionNameForDebugging(uint nameIndex)
                    => SectionNameTable?.GetString(nameIndex) ?? "<Unknown>";

                TSection? GetLinkedSection<TSection>(ElfSectionHeader header)
                    where TSection : ElfSection
                {
                    if (header.Link == 0)
                    { return null; }

                    ElfSection linkedSection = ParseSection(checked((int)header.Link));
                    if (linkedSection is TSection result)
                    { return result; }

                    throw new MalformedFileException
                    (
                        $"Symbol table '{GetSectionNameForDebugging(header.NameIndex)}' references section #{header.Link} as a {typeof(TSection).Name}, but it's a {linkedSection.GetType().Name}.",
                        header.HeaderStart
                    );
                }

                ElfSection ParseSection(int sectionIndex)
                {
                    if (sectionsBuilder[sectionIndex] is not null)
                    { return sectionsBuilder[sectionIndex]; }

                    ElfSectionHeader header = sectionHeaders[sectionIndex];
                    ElfSection section;
                    switch (header.Type)
                    {
                        case ElfSectionType.StringTable:
                            section = new ElfStringTableSection(this, header, stream, sectionIndex);
                            break;
                        case ElfSectionType.SymbolTable:
                        case ElfSectionType.DynamicSymbolTable:
                        {
                            ElfStringTableSection? symbolStringTable = GetLinkedSection<ElfStringTableSection>(header);
                            section = new ElfSymbolTableSection(this, header, stream, sectionIndex, symbolStringTable);
                        }
                        break;
                        case ElfSectionType.NoData:
                            section = new ElfNoDataSection(this, header, sectionIndex);
                            break;
                        default:
                            section = new ElfUnstructuredSection(this, header, sectionIndex);
                            break;
                    };

                    Debug.Assert(sectionsBuilder[sectionIndex] is null);
                    sectionsBuilder[sectionIndex] = section;
                    return section;
                }

                for (int i = 0; i < sectionHeaders.Length; i++)
                {
                    // If this section was already parsed (because it was required to parse another section), skip it
                    if (sectionsBuilder[i] is not null)
                    { continue; }

                    ElfSection newSection = ParseSection(i);

                    switch (newSection)
                    {
                        // Save the symbol tables as they're created
                        // "Currently, an object file may have only one section of each type"
                        // https://refspecs.linuxfoundation.org/elf/gabi4+/ch4.sheader.html#sh_type
                        case ElfSymbolTableSection { Type: ElfSectionType.SymbolTable } newSymbolTable:
                            Debug.Assert(SymbolTable is null, "File has more than one symbol table!");
                            SymbolTable ??= newSymbolTable;
                            break;
                        case ElfSymbolTableSection { Type: ElfSectionType.DynamicSymbolTable } newDynamicSymbolTable:
                            Debug.Assert(DynamicSymbolTable is null, "File has more than one dynamic symbol table!");
                            DynamicSymbolTable ??= newDynamicSymbolTable;
                            break;
                    }
                }
                Sections = sectionsBuilder.MoveToImmutable();
            }
        }

        public ImmutableArray<ElfSection>.Enumerator GetEnumerator()
            => Sections.GetEnumerator();
        IEnumerator<ElfSection> IEnumerable<ElfSection>.GetEnumerator()
            => ((IEnumerable<ElfSection>)Sections).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()
            => ((IEnumerable)Sections).GetEnumerator();

        public static bool IsElfFile(Stream stream)
            => ElfIdentity.IsElfFile(stream);
    }
}

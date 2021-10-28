using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;

namespace Kaisa.Elf
{
    public sealed class ElfArchiveMember : ArchiveMember, IEnumerable<ElfSection>
    {
        public ElfFile ElfFile { get; }
        public ElfHeader Header => ElfFile.Header;
        public ImmutableArray<ElfSection> Sections => ElfFile.Sections;

        internal ElfArchiveMember(Archive library, ArchiveMemberHeader header, ushort sig1, ushort sig2, Stream stream)
            : base(library, header)
        {
            ElfFile = new ElfFile(sig1, sig2, stream);

            // The ELF file format is not linear, so the cursor is left in a random position after it's done reading.
            stream.Position = MemberEnd;
        }

        public ImmutableArray<ElfSection>.Enumerator GetEnumerator()
            => ElfFile.GetEnumerator();
        IEnumerator<ElfSection> IEnumerable<ElfSection>.GetEnumerator()
            => ((IEnumerable<ElfSection>)ElfFile).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()
            => ((IEnumerable)ElfFile).GetEnumerator();
    }
}

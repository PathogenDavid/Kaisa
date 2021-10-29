using System;
using System.IO;

namespace Kaisa.Elf
{
    internal sealed class ElfSymbolTableExtendedIndicesSection : ElfSection
    {
        private readonly uint[] _SectionIndices;
        internal ReadOnlySpan<uint> SectionIndices => _SectionIndices;

        internal ElfSymbolTableExtendedIndicesSection(ElfFile file, ElfSectionHeader header, Stream stream, int index)
            : base(file, header, index)
        {
            stream.Position = DataStart;
            ulong indexCount = DataLength / sizeof(uint);

            if (indexCount >= int.MaxValue)
            { throw new NotSupportedException($"Kaisa cannot decode symbol table extended indices section with {indexCount} entries."); }

            _SectionIndices = stream.Read<uint>(checked((int)indexCount));
        }
    }
}

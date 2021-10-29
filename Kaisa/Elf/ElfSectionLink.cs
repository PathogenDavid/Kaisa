using System;
using System.Diagnostics;

namespace Kaisa.Elf
{
    internal readonly struct ElfSectionLink
    {
        private readonly ElfFile File;
        // Negative values indicate a reserved section index when initialized using a ushort index
        private readonly int SectionIndex;

        private const int NullSectionIndex = -1;

        private ElfSectionLink(ElfFile file, uint sectionIndex)
        {
            File = file;

            if (sectionIndex > int.MaxValue)
            { throw new NotSupportedException($"Kaisa does not support referencing a section #{sectionIndex}"); }

            SectionIndex = checked((int)sectionIndex);
        }

        private ElfSectionLink(ElfFile file, ushort sectionIndex)
        {
            File = file;
            SectionIndex = sectionIndex;

            if (SectionIndex >= ElfSection.SHN_LORESERVE)
            { SectionIndex = -SectionIndex; }
        }

        private ElfSectionLink(ElfFile file)
        {
            File = file;
            SectionIndex = NullSectionIndex;
        }

        internal static ElfSectionLink MakeIntegerLink(ElfFile file, uint sectionIndex)
            => new ElfSectionLink(file, sectionIndex);

        internal static ElfSectionLink MakeShortLink(ElfFile file, ushort sectionIndex)
            => new ElfSectionLink(file, sectionIndex);

        internal static ElfSectionLink MakeNullLink(ElfFile file)
            => new ElfSectionLink(file);

        public ElfSection? Section
        {
            get
            {
                Debug.Assert(File is not null, "Tried to resolve an uninitialized section link.");
                Debug.Assert(!File.Sections.IsDefault, "Tried to resolve a section link in a file that is not fully initialized.");

                if (SectionIndex == NullSectionIndex)
                { return null; }

                if (SectionIndex < 0)
                { throw new NotSupportedException($"Tried to resolve section 0x{-SectionIndex:X4}, which has special meaning not understood by Kaisa."); }

                return File.Sections[SectionIndex];
            }
        }

        public static implicit operator ElfSection?(ElfSectionLink link)
            => link.Section;

        // We provide a ToString which doesn't assert or throw to make debugging more pleasant when the link is in a bad state or the file is uninitialized
        public override string ToString()
        {
            if (File is null)
            { return "<Uninitialized Link>"; }
            else if (SectionIndex < 0)
            { return $"<Special:0x{-SectionIndex:X4}>"; }
            else if (File.Sections.IsDefault)
            { return $"<Section#{SectionIndex}>"; }
            else
            { return Section?.Name ?? $"<UnnamedSection#{SectionIndex}>"; }
        }
    }
}

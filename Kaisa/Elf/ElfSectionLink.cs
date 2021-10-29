using System;
using System.Diagnostics;

namespace Kaisa.Elf
{
    internal readonly struct ElfSectionLink
    {
        private readonly ElfFile File;
        private readonly ushort SectionIndex;

        public ElfSectionLink(ElfFile file, ushort sectionIndex)
        {
            File = file;
            SectionIndex = sectionIndex;
        }

        public ElfSection Section
        {
            get
            {
                Debug.Assert(File is not null, "Tried to resolve an uninitialized section link.");
                Debug.Assert(!File.Sections.IsDefault, "Tried to resolve a section link in a file that is not fully initialized.");

                if (SectionIndex >= ElfSection.SHN_LORESERVE)
                { throw new NotSupportedException($"Tried to resolve section 0x{SectionIndex:X4}, which has special meaning not understood by Kaisa."); }

                return File.Sections[SectionIndex];
            }
        }

        public static implicit operator ElfSection(ElfSectionLink link)
            => link.Section;

        // We provide a ToString which doesn't assert or throw to make debugging more pleasant when the link is in a bad state or the file is uninitialized
        public override string ToString()
        {
            if (File is null)
            { return "<Uninitialized Link>"; }
            else if (SectionIndex >= ElfSection.SHN_LORESERVE)
            { return $"<Special:0x{SectionIndex:X4}>"; }
            else if (File.Sections.IsDefault)
            { return $"<Section#{SectionIndex}>"; }
            else
            { return Section.Name ?? $"<Section#{SectionIndex}>"; }
        }
    }
}

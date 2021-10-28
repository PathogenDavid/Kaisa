using System;
using System.Diagnostics;

namespace Kaisa
{
    public abstract class ArchiveMember
    {
        public Archive Library { get; }
        public string RawName { get; }
        public string Name { get; }
        public int Size { get; }

        public long MemberStart { get; }
        public long MemberDataStart { get; }
        public long MemberEnd { get; }

        protected ArchiveMember(Archive library, ArchiveMemberHeader header)
        {
            Library = library;
            RawName = header.Name;
            Size = header.Size;

            MemberStart = header.MemberStart;
            MemberDataStart = header.MemberDataStart;
            MemberEnd = header.MemberEnd;

            // Determine the non-raw name
            if (RawName == "/")
            {
                Name = "<Index>";
                Debug.Assert(this is ArchiveIndexV1 || this is ArchiveIndexV2);
            }
            else if (RawName == "//")
            {
                Name = "<Longnames>";
                Debug.Assert(this is ArchiveLongnames);
            }
            else if (RawName.StartsWith('/'))
            {
                if (library.Longnames is null)
                { throw new ArgumentException($"The header is malformed: Header references a longname when the library has no longnames file.", nameof(header)); }

                int longnameOffset;
                if (!Int32.TryParse(RawName.AsSpan().Slice(1), out longnameOffset))
                { throw new ArgumentException($"The header is malformed: Could not parse longname offset '{RawName}'.", nameof(header)); }

                Name = library.Longnames.GetLongname(longnameOffset);

                // Unlike the short name, the longname on Windows does not have a terminating slash
                // On Linux it does: https://refspecs.linuxfoundation.org/elf/gabi41.pdf#page=153
                // "[...] a table of file names, each followed by a slash [...]"
                if (Name.EndsWith('/'))
                { Name = Name.Substring(0, Name.Length - 1); }
            }
            else if (RawName.EndsWith('/'))
            { Name = RawName.Substring(0, RawName.Length - 1); }
            else
            {
                Debug.Fail($"Malformed name '{RawName}'"); // Don't throw an exception and assume this file is just a new one we don't know about.
                Name = RawName;
            }
        }

        public override string ToString()
            => $"{GetType().Name}: '{Name}' @ 0x{MemberStart:X},0x{MemberDataStart:X}..{MemberEnd:X} ({Size} bytes)";
    }
}

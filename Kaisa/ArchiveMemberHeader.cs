using System;
using System.IO;

namespace Kaisa
{
    // The format of this header is the same between Windows and Linux.
    // Windows: https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#archive-member-headers
    // Linux: https://refspecs.linuxfoundation.org/elf/gabi41.pdf#page=152
    public struct ArchiveMemberHeader
    {
        public string Name { get; }
        public int Size { get; }
        public long MemberStart { get; }
        public long MemberDataStart { get; }
        public long MemberEnd { get; }

        public ArchiveMemberHeader(Stream stream)
        {
            MemberStart = stream.Position;

            // "each field is an ASCII text string that is left justified and padded with spaces to the end of the field"
            Name = stream.ReadAscii(16).TrimEnd(' ');
            stream.ReadAscii(12); // Skip date (seemingly no longer populated)
            stream.ReadAscii(6); // Skip user ID (Documented as not populated by Microsoft tools)
            stream.ReadAscii(6); // Skip group ID (Documented as not populated by Microsoft tools)
            stream.ReadAscii(8); // Skip mode

            long sizeStringOffset = stream.Position;
            string sizeString = stream.ReadAscii(10).TrimEnd(' ');
            int size;

            if (!Int32.TryParse(sizeString, out size))
            { throw new MalformedFileException($"Could not archive member size '{sizeString}' as an integer.", sizeStringOffset); }

            Size = size;

            long endOfHeaderMagicOffset = stream.Position;
            string endOfHeaderMagic = stream.ReadAscii(2);
            if (endOfHeaderMagic != "`\n")
            { throw new MalformedFileException("Archive member end of header magic was incorrect.", endOfHeaderMagicOffset); }

            MemberDataStart = stream.Position;
            MemberEnd = MemberDataStart + Size;
        }
    }
}

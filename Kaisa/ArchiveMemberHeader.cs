using System;
using System.IO;

namespace Kaisa
{
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

            string sizeString = stream.ReadAscii(10).TrimEnd(' ');
            int size;

            if (!Int32.TryParse(sizeString, out size))
            { throw new ArgumentException($"The archive is malformed: Could not parse size member '{sizeString}' as an integer."); }

            Size = size;

            string endOfHeaderMagic = stream.ReadAscii(2);
            if (endOfHeaderMagic != "`\n")
            { throw new ArgumentException("The archive is malformed: End of header magic was incorrect."); }

            MemberDataStart = stream.Position;
            MemberEnd = MemberDataStart + Size;
        }
    }
}

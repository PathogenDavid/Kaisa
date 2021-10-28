using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Kaisa
{
    // Windows: https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#longnames-member
    // Linux: https://refspecs.linuxfoundation.org/elf/gabi41.pdf#page=153
    // On Linux this is called the string table member instead of the longnames member.
    public sealed class ArchiveLongnames : ArchiveMember
    {
        private readonly byte[] Data;

        internal ArchiveLongnames(Archive library, ArchiveMemberHeader header, Stream stream)
            : base(library, header)
            => Data = stream.Read<byte>(Size);

        internal ArchiveVariant GuessVariant()
        {
            foreach (byte b in Data)
            {
                // In Windows-style archives, longnames are null-terminated.
                // In Linux-style archives they are newline-terminated.
                // We don't expect nulls to appear in Linux string tables or newlines to appear in Windows longname tables.
                if (b == 0)
                { return ArchiveVariant.Windows; }
                else if (b == '\n')
                { return ArchiveVariant.Linux; }
            }

            Debug.Fail("We always expect to be able to guess the variant from the longnames file.");
            return ArchiveVariant.Unknown;
        }

        public string GetLongname(int offset)
        {
            if (offset < 0 || offset > Data.Length)
            { throw new ArgumentOutOfRangeException(nameof(offset)); }

            int end = offset;
            // In Windows-style archives, longnames are null-terminated.
            // In Linux-style archives they are newline-terminated.
            // We don't expect nulls to appear in Linux string tables or newlines to appear in Windows longname tables.
            for (; end < Data.Length && Data[end] != 0 && Data[end] != '\n'; end++)
            { }

            if (end == Data.Length)
            { throw new MalformedFileException($"Longname at {offset} is not terminated!", MemberDataStart + offset); }

            switch (Data[end])
            {
                case 0:
                    Debug.Assert(Library.Variant is ArchiveVariant.Windows, "Nulls should only appear in the Windows variant of the longnames file.");
                    break;
                case (byte)'\n':
                    Debug.Assert(Library.Variant is ArchiveVariant.Linux, "Nulls should only appear in the Linux variant of the longnames file.");
                    break;
                default:
                    Debug.Fail("Should not reach here.");
                    break;
            }

            int length = end - offset;
            return Encoding.ASCII.GetString(Data, offset, length);
        }
    }
}

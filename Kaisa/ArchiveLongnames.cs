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
            for (; Data[end] != 0 && Data[end] != '\n'; end++)
            { }

            int length = end - offset;
            return Encoding.ASCII.GetString(Data, offset, length);
        }
    }
}

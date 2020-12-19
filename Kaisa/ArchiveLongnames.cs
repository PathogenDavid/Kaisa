using System;
using System.IO;
using System.Text;

namespace Kaisa
{
    public sealed class ArchiveLongnames : ArchiveMember
    {
        private readonly byte[] Data;

        internal ArchiveLongnames(Archive library, ArchiveMemberHeader header, Stream stream)
            : base(library, header)
            => Data = stream.Read<byte>(Size);

        public string GetLongname(int offset)
        {
            if (offset < 0 || offset > Data.Length)
            { throw new ArgumentOutOfRangeException(nameof(offset)); }

            int end = offset;
            for (; Data[end] != 0; end++)
            { }

            int length = end - offset;
            return Encoding.ASCII.GetString(Data, offset, length);
        }
    }
}

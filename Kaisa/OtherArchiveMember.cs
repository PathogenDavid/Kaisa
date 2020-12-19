using System;
using System.IO;

namespace Kaisa
{
    public sealed class OtherArchiveMember : ArchiveMember
    {
        public ReadOnlyMemory<byte> Data { get; }

        internal OtherArchiveMember(Archive library, ArchiveMemberHeader header, Stream stream)
            : base(library, header)
            => Data = stream.Read<byte>(Size);
    }
}

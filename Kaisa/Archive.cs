using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;

namespace Kaisa
{
    public sealed class Archive : IEnumerable<ArchiveMember>
    {
        public ArchiveIndexV1 IndexV1 { get; }
        public ArchiveIndexV2? IndexV2 { get; }
        public ArchiveLongnames? Longnames { get; }
        public ImmutableArray<ArchiveMember> ObjectFiles { get; }

        public Archive(Stream stream)
        {
            string expectedFileSignature = "!<arch>\n";
            if (stream.ReadAscii(expectedFileSignature.Length) != expectedFileSignature)
            { throw new ArgumentException("The library is malformed: Invalid file signature.", nameof(stream)); }

            ImmutableArray<ArchiveMember>.Builder filesBuilder = ImmutableArray.CreateBuilder<ArchiveMember>();

            for (int i = 0; stream.Position < stream.Length; i++)
            {
                ArchiveMemberHeader header = new(stream);
                long expectedEnd = stream.Position + header.Size;

                if (i == 0)
                {
                    if (header.Name != "/")
                    { throw new ArgumentException("The library is malformed: Missing index file.", nameof(stream)); }

                    IndexV1 = new ArchiveIndexV1(this, header, stream);
                }
                else if (i == 1 && header.Name == "/")
                { IndexV2 = new ArchiveIndexV2(this, header, stream); }
                else if (header.Name == "//")
                {
                    if (Longnames is not null)
                    { throw new ArgumentException("The library is malformed: Library contains multiple longnames files.", nameof(stream)); }

                    Longnames = new ArchiveLongnames(this, header, stream);
                }
                else
                {
                    ushort sig1 = stream.Read<ushort>();
                    ushort sig2 = stream.Read<ushort>();

                    if (ImportObjectHeader.IsImportObject(sig1, sig2))
                    { filesBuilder.Add(new ImportArchiveMember(this, header, new ImportObjectHeader(sig1, sig2, stream), stream)); }
                    else if (CoffHeader.IsMaybeCoffObject(sig1, sig2))
                    { filesBuilder.Add(new CoffArchiveMember(this, header, new CoffHeader(sig1, sig2, stream), stream)); }
                    else
                    {
                        filesBuilder.Add(new UnknownArchiveMember(this, header));
                        stream.Position += header.Size - (sizeof(ushort) * 2);
                    }
                }

                if (stream.Position != expectedEnd)
                {
                    Debug.Fail($"File @ {i} {(stream.Position > expectedEnd ? "underran" : "overran")} its span!");
                    stream.Position = expectedEnd;
                }

                // If we're on an odd address, advance by 1 to get to an even address
                // "Each member header starts on the first even address after the end of the previous archive member."
                // (We do this here because the final file in the archive may have padding after it for the next file which doesn't exist.)
                if (stream.Position % 2 == 1)
                { stream.Position++; }
            }

            if (IndexV1 is null)
            { throw new ArgumentException("The library is malformed: Library is empty.", nameof(stream)); }

            filesBuilder.Capacity = filesBuilder.Count;
            ObjectFiles = filesBuilder.MoveToImmutable();
        }

        public IEnumerator<ArchiveMember> GetEnumerator()
        {
            yield return IndexV1;

            if (IndexV2 is not null)
            { yield return IndexV2; }

            if (Longnames is not null)
            { yield return Longnames; }

            foreach (ArchiveMember objectFile in ObjectFiles)
            { yield return objectFile; }
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}

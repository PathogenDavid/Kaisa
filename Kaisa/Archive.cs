using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;

namespace Kaisa
{
    // Windows: https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#archive-library-file-format
    // Linux: https://refspecs.linuxfoundation.org/elf/gabi41.pdf#page=152
    public sealed class Archive : IEnumerable<ArchiveMember>
    {
        private ArchiveVariant _Variant = ArchiveVariant.Unknown;
        /// <summary>Our best guess at the variant of the archive file format this file was in.</summary>
        public ArchiveVariant Variant
        {
            get => _Variant;
            init
            {
                if (_Variant == ArchiveVariant.Unknown)
                { _Variant = value; }
                else if (_Variant != value)
                { Debug.Fail($"Guessed variant as {value} but variant was already guessed as {_Variant}!"); }
            }
        }

        public ArchiveIndexV1 IndexV1 { get; }
        public ArchiveIndexV2? IndexV2 { get; }
        public ArchiveLongnames? Longnames { get; }
        public ImmutableArray<ArchiveMember> ObjectFiles { get; }

        public Archive(Stream stream)
        {
            if (!IsHeaderValid(stream))
            { throw new MalformedFileException("The specified stream does not appear to represent a valid archive file, the signature is invalid.", stream.Position - ExpectedFileSignature.Length); }

            ImmutableArray<ArchiveMember>.Builder filesBuilder = ImmutableArray.CreateBuilder<ArchiveMember>();

            for (int i = 0; stream.Position < stream.Length; i++)
            {
                ArchiveMemberHeader header = new(stream);
                long expectedEnd = stream.Position + header.Size;

                // The symbol index file is defined as always being first on both Windows and Linux.
                if (i == 0)
                {
                    // Note that archive files can be used for things other than static/import libraries (IE: .deb packages),
                    // but we don't support them so this exception isn't unreasonable.
                    if (header.Name != "/")
                    { throw new MalformedFileException("The first file is not a symbol index file.", header.MemberStart); }

                    IndexV1 = new ArchiveIndexV1(this, header, stream);
                }
                // This file is only ever present in Windows-style archives and is always second.
                else if (i == 1 && header.Name == "/")
                {
                    IndexV2 = new ArchiveIndexV2(this, header, stream);
                    Variant = ArchiveVariant.Windows;
                }
                // The longnames file might not be present if none of the files have names exceeding 15 bytes.
                // It is expected to come after the symbol index file(s) and before any other "normal" files.
                else if (header.Name == "//")
                {
                    Debug.Assert(i == 1 || i == 2, "The longnames file should always come after the symbol index file(s) and before other files.");

                    if (Longnames is not null)
                    { throw new MalformedFileException("Library contains multiple longnames files.", header.MemberStart); }

                    Longnames = new ArchiveLongnames(this, header, stream);
                    Variant = Longnames.GuessVariant();
                }
                else
                {
                    ushort sig1 = stream.Read<ushort>();
                    ushort sig2 = stream.Read<ushort>();

                    if (ImportObjectHeader.IsImportObject(sig1, sig2))
                    {
                        filesBuilder.Add(new ImportArchiveMember(this, header, new ImportObjectHeader(sig1, sig2, stream), stream));
                        Variant = ArchiveVariant.Windows;
                    }
                    else if (CoffHeader.IsMaybeCoffObject(sig1, sig2))
                    {
                        filesBuilder.Add(new CoffArchiveMember(this, header, new CoffHeader(sig1, sig2, stream), stream));
                        Variant = ArchiveVariant.Windows;
                    }
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
            { throw new MalformedFileException("The library is malformed: Library is empty.", stream.Position); }

            filesBuilder.Capacity = filesBuilder.Count;
            ObjectFiles = filesBuilder.MoveToImmutable();
        }

        public static bool IsArchiveFile(Stream stream)
        {
            if (!stream.CanSeek)
            { throw new NotSupportedException("Cannot check streams that are not seekable."); }

            long oldPosition = stream.Position;
            bool result = IsHeaderValid(stream);
            stream.Position = oldPosition;
            return result;
        }

        private const string ExpectedFileSignature = "!<arch>\n";
        private static bool IsHeaderValid(Stream stream)
            => stream.ReadAscii(ExpectedFileSignature.Length) == ExpectedFileSignature;

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

using System.IO;

namespace Kaisa
{
    // https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#import-library-format
    public sealed class ImportArchiveMember : ArchiveMember
    {
        public ImportObjectHeader ImportHeader { get; }
        public string Symbol { get; }
        public string Dll { get; }

        public ImportArchiveMember(Archive library, ArchiveMemberHeader header, ImportObjectHeader importHeader, Stream stream)
            : base(library, header)
        {
            ImportHeader = importHeader;
            // The documentation does not state the encoding of these strings, but in practice they appear to be UTF8.
            Symbol = stream.ReadUtf8NullTerminated();
            Dll = stream.ReadUtf8NullTerminated();
        }

        public override string ToString()
            => $"{base.ToString()} (Symbol: {Symbol}) (Dll: {Dll}) (Type: {ImportHeader.Type}, {ImportHeader.NameType})" +
            $" ({(ImportHeader.NameType == ImportNameType.Ordinal ? "Ordinal" : "Hint")}: {ImportHeader.OrdinalOrHint})";
    }
}

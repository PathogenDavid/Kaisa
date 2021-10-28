using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;

namespace Kaisa
{
    public sealed class CoffArchiveMember : ArchiveMember
    {
        public CoffHeader CoffHeader { get; }
        public ImmutableArray<SectionHeader> SectionHeaders { get; }
        public ImmutableArray<CoffSymbol> Symbols { get; } = ImmutableArray<CoffSymbol>.Empty;

        internal unsafe CoffArchiveMember(Archive library, ArchiveMemberHeader header, CoffHeader coffHeader, Stream stream)
            : base(library, header)
        {
            long expectedEnd = stream.Position + Size - sizeof(CoffHeader);

            CoffHeader = coffHeader;

            // If the optional header is present, skip over it
            // (The documentation is conflicted on whether or not this header can be present, but it does state that if it is present it is useless so we don't try to read it if it's there.)
            stream.Position += CoffHeader.SizeOfOptionalHeader;

            ImmutableArray<SectionHeader>.Builder sectionHeaders = ImmutableArray.CreateBuilder<SectionHeader>(CoffHeader.NumberOfSections);
            for (ushort i = 0; i < CoffHeader.NumberOfSections; i++)
            {
                sectionHeaders.Add(new SectionHeader(stream));
            }
            SectionHeaders = sectionHeaders.MoveToImmutable();

            // Read in symbols (if present)
            if (CoffHeader.PointerToSymbolTable != 0)
            {
                long symbolTableStart = MemberDataStart + CoffHeader.PointerToSymbolTable;
                CoffStringTable stringTable;

                // Read in the string table first so that it can be used if needed
                {
                    stream.Position = symbolTableStart + CoffHeader.NumberOfSymbols * CoffSymbol.NativeSymbolHeaderSize;
                    stringTable = new CoffStringTable(stream);
                }

                // Read in the symbol table
                {
                    stream.Position = symbolTableStart;
                    ImmutableArray<CoffSymbol>.Builder symbols = ImmutableArray.CreateBuilder<CoffSymbol>(checked((int)CoffHeader.NumberOfSymbols));
                    for (uint i = 0; i < CoffHeader.NumberOfSymbols; i++)
                    {
                        CoffSymbol symbol = new(stream, stringTable);
                        symbols.Add(symbol);

                        // Axillary symbols are included in the total count but were processed by CoffSymbol
                        i += symbol.NumberOfAuxSymbols;
                    }
                    symbols.Capacity = symbols.Count;
                    Symbols = symbols.MoveToImmutable();
                }
            }

            // We might not read everything a COFF file has to offer so just skip to the end (but make sure we didn't overrun it first.)
            Debug.Assert(stream.Position <= expectedEnd, "COFF file parsing overran the end of the file!");
            stream.Position = expectedEnd;
        }

        public override string ToString()
            => $"{base.ToString()} (MachineType: {CoffHeader.Machine}) (Sections: {CoffHeader.NumberOfSections}) (SymbolCount: {CoffHeader.NumberOfSymbols}) (Characteristics: {CoffHeader.Characteristics})";
    }
}

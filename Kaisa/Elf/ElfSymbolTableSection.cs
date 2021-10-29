using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;

namespace Kaisa.Elf
{
    // https://refspecs.linuxfoundation.org/elf/gabi4+/ch4.symtab.html
    public sealed class ElfSymbolTableSection : ElfSection, IEnumerable<ElfSymbol>
    {
        public ImmutableArray<ElfSymbol> Symbols { get; }

        public ElfSymbolTableSection(ElfFile file, ElfSectionHeader header, Stream stream, int index, ElfStringTableSection? symbolStringTable)
            : base(file, header, index)
        {
            // Validate the symbol string table
            if (header.Link == 0 && symbolStringTable is not null)
            { throw new ArgumentException($"The header describes a symbol table without an associated string table, but string table #{symbolStringTable.Index} provided.", nameof(symbolStringTable)); }
            else if (symbolStringTable is null)
            { throw new ArgumentNullException(nameof(symbolStringTable), $"The header designates string table #{header.Link} as the symbol string table, but none was provided."); }
            else if (header.Link != symbolStringTable.Index)
            { throw new ArgumentException($"The header designates string table #{header.Link} as the symbol string table, but string table #{symbolStringTable.Index} was provided instead.", nameof(symbolStringTable)); }
            else if (symbolStringTable.File != File)
            { throw new ArgumentException("The specified symbol string table doesn't belong to the same ELF file as this symbol table.", nameof(symbolStringTable)); }

            ushort symbolSize = file.Is64Bit ? ElfSymbol.Size64 : ElfSymbol.Size32;
            if ((DataLength % symbolSize) != 0)
            { throw new MalformedFileException($"Symbol table is {DataLength} bytes long, which is not a multiple of the symbol length ({symbolSize} bytes.)", DataStart); }

            stream.Position = DataStart;
            ulong symbolCount = DataLength / symbolSize;

            if (symbolCount >= int.MaxValue)
            { throw new NotSupportedException($"Kaisa cannot decode symbol tables with {symbolCount} entries."); }

            ImmutableArray<ElfSymbol>.Builder symbolsBuilder = ImmutableArray.CreateBuilder<ElfSymbol>(checked((int)symbolCount));
            for (ulong i = 0; i < symbolCount; i++)
            { symbolsBuilder.Add(new ElfSymbol(file, stream, symbolStringTable)); }

            Symbols = symbolsBuilder.MoveToImmutable();

            Debug.Assert(stream.Position == DataStart + checked((long)DataLength), "Parsing the symbol table should exhaust the section data.");
        }

        public ImmutableArray<ElfSymbol>.Enumerator GetEnumerator()
            => Symbols.GetEnumerator();
        IEnumerator<ElfSymbol> IEnumerable<ElfSymbol>.GetEnumerator()
            => ((IEnumerable<ElfSymbol>)Symbols).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()
            => ((IEnumerable)Symbols).GetEnumerator();
    }
}

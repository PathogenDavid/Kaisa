using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;

namespace Kaisa
{
    public sealed class ArchiveIndexV2 : ArchiveMember
    {
        public ImmutableDictionary<string, uint> Symbols { get; }
        public ImmutableArray<(string, uint)> AllSymbols { get; }

        internal ArchiveIndexV2(Archive library, ArchiveMemberHeader header, Stream stream)
            : base(library, header)
        {
            long expectedEnd = stream.Position + Size;

            // Members
            uint memberCount = stream.Read<uint>();
            uint[] offsets = new uint[memberCount];

            for (uint i = 0; i < memberCount; i++)
            { offsets[i] = stream.Read<uint>(); }

            // Symbols
            uint symbolCount = stream.Read<uint>();
            ushort[] indices = new ushort[symbolCount];

            for (uint i = 0; i < symbolCount; i++)
            { indices[i] = stream.Read<ushort>(); }

            ImmutableDictionary<string, uint>.Builder symbolsBuilder = ImmutableDictionary.CreateBuilder<string, uint>();
            ImmutableArray<(string, uint)>.Builder allSymbolsBuilder = ImmutableArray.CreateBuilder<(string, uint)>(checked((int)symbolCount));

            for (uint i = 0; i < symbolCount; i++)
            {
                string symbol = stream.ReadAsciiNullTerminated();
                uint offset = offsets[indices[i] - 1];

                allSymbolsBuilder.Add((symbol, offset));

                // Some libraries have duplicate symbols, so we only store the first one in the lookup dictionary.
                if (!symbolsBuilder.ContainsKey(symbol))
                { symbolsBuilder.Add(symbol, offset); }
            }

            Symbols = symbolsBuilder.ToImmutable();
            AllSymbols = allSymbolsBuilder.MoveToImmutable();

            Debug.Assert(stream.Position == expectedEnd);
        }

        public override string ToString()
            => $"{base.ToString()} ({AllSymbols.Length} symbols)";
    }
}

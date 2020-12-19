using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;

namespace Kaisa
{
    public sealed class ArchiveIndexV1 : ArchiveMember
    {
        public ImmutableDictionary<string, uint> Symbols { get; }
        public ImmutableArray<(string, uint)> AllSymbols { get; }

        internal ArchiveIndexV1(Archive library, ArchiveMemberHeader header, Stream stream)
            : base(library, header)
        {
            long expectedEnd = stream.Position + Size;
            
            uint symbolCount = BitHelper.FromBigEndian(stream.Read<uint>());
            uint[] offsets = new uint[symbolCount];

            for (uint i = 0; i < symbolCount; i++)
            { offsets[i] = BitHelper.FromBigEndian(stream.Read<uint>()); }

            ImmutableDictionary<string, uint>.Builder symbolsBuilder = ImmutableDictionary.CreateBuilder<string, uint>();
            ImmutableArray<(string, uint)>.Builder allSymbolsBuilder = ImmutableArray.CreateBuilder<(string, uint)>(checked((int)symbolCount));

            for (uint i = 0; i < symbolCount; i++)
            {
                string symbol = stream.ReadAsciiNullTerminated();
                uint offset = offsets[i];

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

using System;
using System.IO;
using System.Text;

namespace Kaisa
{
    // https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#coff-symbol-table
    public struct CoffSymbol
    {
        public string Name { get; }
        public uint Value { get; }
        /// <summary>1-index section number</summary>
        /// <remarks>
        /// Can have the following special values:
        /// 
        /// * IMAGE_SYM_UNDEFINED (0): The symbol record is not yet assigned a section. A value of zero indicates that a reference to an external symbol is defined elsewhere.
        ///     A value of non-zero is a common symbol with a size that is specified by the value.
        /// * IMAGE_SYM_ABSOLUTE (-1): The symbol has an absolute (non-relocatable) value and is not an address.
        /// * IMAGE_SYM_DEBUG (-2): The symbol provides general type or debugging information but does not correspond to a section.
        ///     Microsoft tools use this setting along with .file records (storage class FILE).
        ///     
        /// (From https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#section-number-values)
        /// </remarks>
        public short SectionNumber { get; }
        public CoffSymbolBaseType BaseType { get; }
        public CoffSymbolComplexType ComplexType { get; }
        public CoffSymbolStorageClass StorageClass { get; }
        public byte NumberOfAuxSymbols { get; }

        public const int NativeSymbolHeaderSize = 18;

        public CoffSymbol(Stream stream, CoffStringTable stringTable)
        {
            Span<byte> nameBytes = stackalloc byte[8];
            stream.Read<byte>(nameBytes);

            if (nameBytes[0] == 0 && nameBytes[1] == 0 && nameBytes[2] == 0 && nameBytes[3] == 0)
            {
                uint nameOffset = BitConverter.ToUInt32(nameBytes.Slice(4));
                Name = stringTable.GetString(checked((int)nameOffset));
            }
            else
            { Name = Encoding.UTF8.GetString(nameBytes).TrimEnd('\0'); }

            Value = stream.Read<uint>();
            SectionNumber = stream.Read<short>();
            ComplexType = stream.Read<CoffSymbolComplexType>();
            BaseType = stream.Read<CoffSymbolBaseType>();
            StorageClass = stream.Read<CoffSymbolStorageClass>();
            NumberOfAuxSymbols = stream.Read<byte>();

            // Skip auxillary symbols
            stream.Position += NumberOfAuxSymbols * NativeSymbolHeaderSize;
        }

        public override string ToString()
            => $"'{Name}' = 0x{Value:X}, Section: {SectionNumber}, BaseType: {BaseType}, ComplexType: {ComplexType}, StorageClass: {StorageClass}, AuxSymbols: {NumberOfAuxSymbols}";
    }
}

using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Kaisa.Elf
{
    // https://refspecs.linuxfoundation.org/elf/gabi4+/ch4.strtab.html
    /// <summary>Represents a string table. (That is, its <see cref="ElfSectionHeader.Type"/> is <see cref="ElfSectionType.StringTable"/>.)</summary>
    public sealed class StringTableSection : ElfSection
    {
        private readonly byte[] Data;

        private StringTableSection(ElfFile file, ElfSectionHeader header, Stream stream, int index, string? name)
            : base(file, header, index, name)
            => Data = GetData(File, Header, stream);

        internal StringTableSection(ElfFile file, ElfSectionHeader header, Stream stream, int index)
            : base(file, header, index)
            => Data = GetData(File, Header, stream);

        private static byte[] GetData(ElfFile file, ElfSectionHeader header, Stream stream)
        {
            if (header.Size > int.MaxValue)
            { throw new NotSupportedException("Section is too long to be read by Kaisa."); }

            stream.Position = file.FileStartOffset + header.OffsetInFile;
            byte[] data = stream.Read<byte>(checked((int)header.Size));

            Debug.Assert(data.Length == 0 || data[0] == 0, "The section must start with a null if it isn't empty.");
            Debug.Assert(data.Length == 0 || data[data.Length - 1] == 0, "The section must end with a null if it isn't empty.");

            return data;
        }

        public string? GetString(uint offset)
        {
            // The spec is *slightly* weird on empty strings.
            // It's implied that string 0 is a special "none" value, but also that a 0-length string should be null.
            // I feel like it's reasonable for there to be a distinction here, but I think it should be that string 0 is null and a 0-length string is empty so that's what we do.
            // (I would imagine that in practice 0-length strings are probably not common.)
            if (offset == 0)
            { return null; }

            if (offset >= Data.Length)
            { throw new ArgumentOutOfRangeException(nameof(offset)); }

            int end = checked((int)offset);
            for (; end < Data.Length && Data[end] != 0; end++)
            { }

            if (end == Data.Length)
            { throw new MalformedFileException($"String at {offset} is not terminated!", File.FileStartOffset + Header.OffsetInFile + offset); }

            int length = end - (int)offset;

            // The documentation does not state the encoding of the strings other than that they used 8-bits per character
            // We assume UTF-8 to err on the side of caution (although I would imagine it'll never be anything besides ASCII.)
            return Encoding.UTF8.GetString(Data, (int)offset, length);
        }

        internal static StringTableSection CreateSectionNameTable(ElfFile file, ElfSectionHeader header, Stream stream, int index)
        {
            // Figure out our own name if we have one
            // (This is basically why this factory method exists, without it we'd throw an exception in ElfSection's constructor when it tries to discover the name.)
            string? name;
            if (header.NameIndex == 0)
            { name = null; }
            else
            {
                stream.Position = file.FileStartOffset + header.OffsetInFile + header.NameIndex;
                name = stream.ReadUtf8NullTerminated();
            }

            return new StringTableSection(file, header, stream, index, name);
        }
    }
}

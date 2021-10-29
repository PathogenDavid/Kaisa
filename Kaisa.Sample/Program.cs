using Kaisa;
using Kaisa.Elf;
using Kaisa.Sample;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

// Use Unicode output encoding in case any symbols use Unicode
Console.OutputEncoding = Encoding.Unicode;

string? filePath = null;
const string sampleFilePath = @"C:\Program Files (x86)\Windows Kits\10\Lib\10.0.18362.0\um\x64\Shcore.lib";

if (args.Length > 0)
{ filePath = args[0]; }
else if (File.Exists(sampleFilePath))
{ filePath = sampleFilePath; }

[DoesNotReturn]
void ShowUsage(string? error)
{
    TextWriter console = Console.Out;

    if (error is not null)
    {
        Console.Error.WriteLine(error);
        Console.Error.WriteLine();
        console = Console.Error;
    }

    console.WriteLine("Usage:");
    if (File.Exists(sampleFilePath))
    { console.WriteLine("    Kaisa.Sample [file]"); }
    else
    { console.WriteLine("    Kaisa.Sample <file>"); }

    Environment.Exit(error is null ? 0 : 1);
}

if (filePath is null || filePath.ToLowerInvariant() is null or "--help" or "-?" or "/?" or "/help")
{ ShowUsage(null); }
else if (!File.Exists(filePath))
{ ShowUsage($"'{filePath}' does not exist."); }
else if (args.Length > 1)
{ ShowUsage("Too many arguments specified."); }

using FancyConsoleWriter output = new(Console.Out);
using FileStream stream = new(filePath, FileMode.Open, FileAccess.Read);

if (Archive.IsArchiveFile(stream))
{
    output.WriteLine($"Reading '{filePath}' as a library archive...");
    Archive library = new(stream);
    DumpArchive(library);
}
else if (ElfFile.IsElfFile(stream))
{
    output.WriteLine($"Reading '{filePath}' as an ELF object file...");
    ElfFile elf = new(stream);
    DumpElf(elf);
}
else
{ ShowUsage($"Could not detect what type of file '{filePath}' is."); }

void DumpArchive(Archive library)
{
    output.WriteLine($"File guessed to be a {library.Variant}-style archive file.");

    // Print archive members
    foreach (ArchiveMember member in library)
    {
        output.WriteLine(member);
        using (output.Indent())
        {
            if (member is CoffArchiveMember coffMember)
            {
                output.WriteLine("Header:");
                using (output.Indent())
                {
                    output.WriteLine($"Machine: {coffMember.CoffHeader.Machine}");
                    output.WriteLine($"SizeOfOptionalHeader: {coffMember.CoffHeader.SizeOfOptionalHeader}");
                    output.Write("Characteristics: ");
                    {
                        ImageFileCharacteristics characteristics = coffMember.CoffHeader.Characteristics;
                        bool first = true;
                        foreach (ImageFileCharacteristics flag in Enum.GetValues<ImageFileCharacteristics>())
                        {
                            if (flag == 0)
                            { continue; }

                            if ((characteristics & flag) == flag)
                            {
                                if (first)
                                { first = false; }
                                else
                                { output.Write(" | "); }

                                output.Write(flag);
                                characteristics &= ~flag;
                            }
                        }

                        if (characteristics != 0)
                        {
                            if (!first)
                            { output.Write(" | "); }

                            output.Write($"0x{(ushort)characteristics:X}");
                        }
                        else if (first)
                        { output.Write("None"); }
                    }
                    output.WriteLine();
                }

                if (coffMember.SectionHeaders.Length > 0)
                {
                    output.WriteLine("Sections:");
                    using (output.Indent())
                    {
                        foreach (SectionHeader section in coffMember.SectionHeaders)
                        { output.WriteLine(section); }
                    }
                }

                if (coffMember.Symbols.Length > 0)
                {
                    output.WriteLine("Symbols:");
                    using (output.Indent())
                    {
                        foreach (CoffSymbol symbol in coffMember.Symbols)
                        { output.WriteLine(symbol); }
                    }
                }
            }
            else if (member is ElfArchiveMember elfMember)
            { DumpElf(elfMember.ElfFile, skipUnstructured: true); }
        }
    }
}

void DumpElf(ElfFile elf, bool? skipUnstructured = null)
{
    output.WriteLine($"ELF file describes a {elf.Header.Type} file for {elf.Header.Machine} with {elf.Sections.Length} sections.");

    using (output.Indent())
    {
        if (elf.Header.OperatingSystemAbi != ElfOperatingSystemAbi.None)
        { output.WriteLine($"File has platform-specific extensions for {elf.Header.OperatingSystemAbi}"); }

        output.WriteLine($"Symbol table: {(elf.SymbolTable is null ? "Not present" : $"'{elf.SymbolTable.Name ?? "<Unnamed>"}' containing {elf.SymbolTable.Symbols.Length} symbols")}");
        output.WriteLine($"Dynamic symbol table: {(elf.DynamicSymbolTable is null ? "Not present" : $"'{elf.DynamicSymbolTable.Name ?? "<Unnamed>"}' containing {elf.DynamicSymbolTable.Symbols.Length} symbols")}");
    }

    if (elf.Sections.Length is 0)
    { return; }

    if (skipUnstructured is null && elf.Sections.Length > 25)
    { skipUnstructured = true; }

    output.WriteLine("Sections:");
    using (output.Indent())
    {
        bool wroteSection = false;
        foreach (ElfSection section in elf)
        {
            if (skipUnstructured == true && section is ElfUnstructuredSection)
            { continue; }

            output.WriteLine(section);
            wroteSection = true;

            if (section is ElfSymbolTableSection symbolTable)
            {
                using (output.Indent())
                {
                    int i = 0;
                    foreach (ElfSymbol symbol in symbolTable)
                    {
                        if (i > 50)
                        {
                            output.WriteLine("... output truncated ...");
                            break;
                        }

                        output.WriteLine(symbol);
                        i++;
                    }
                }
            }
        }

        if (!wroteSection)
        { output.WriteLine("<All Filtered>"); }
    }
}

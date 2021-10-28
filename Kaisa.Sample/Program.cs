using Kaisa;
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

Console.WriteLine($"Reading '{filePath}'...");
using FileStream stream = new(filePath, FileMode.Open, FileAccess.Read);

if (!Archive.IsArchiveFile(stream))
{ ShowUsage("The specified file does not appear to be a library archive."); }

Archive library = new(stream);
Console.WriteLine($"File guessed to be a {library.Variant}-style archive file.");

// Print archive members
foreach (ArchiveMember member in library)
{
    Console.WriteLine(member);

    if (member is CoffArchiveMember coffMember)
    {
        Console.WriteLine("  Header:");
        Console.WriteLine($"    Machine: {coffMember.CoffHeader.Machine}");
        Console.WriteLine($"    SizeOfOptionalHeader: {coffMember.CoffHeader.SizeOfOptionalHeader}");
        Console.Write($"    Characteristics: ");
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
                    { Console.Write(" | "); }

                    Console.Write(flag);
                    characteristics &= ~flag;
                }
            }

            if (characteristics != 0)
            {
                if (!first)
                { Console.Write(" | "); }

                Console.Write($"0x{(ushort)characteristics:X}");
            }
            else if (first)
            { Console.Write("None"); }
        }
        Console.WriteLine();

        if (coffMember.SectionHeaders.Length > 0)
        {
            Console.WriteLine("  Sections:");
            foreach (SectionHeader section in coffMember.SectionHeaders)
            { Console.WriteLine($"    {section}"); }
        }

        if (coffMember.Symbols.Length > 0)
        {
            Console.WriteLine("  Symbols:");
            foreach (CoffSymbol symbol in coffMember.Symbols)
            { Console.WriteLine($"    {symbol}"); }
        }
    }
}

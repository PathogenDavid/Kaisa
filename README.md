Kaisa the Sharp Librarian
===============================================================================

[![MIT Licensed](https://img.shields.io/github/license/pathogendavid/kaisa?style=flat-square)](LICENSE.txt)
[![CI Status](https://img.shields.io/github/actions/workflow/status/pathogendavid/kaisa/Kaisa.yml?branch=main&style=flat-square&label=CI)](https://github.com/PathogenDavid/Kaisa/actions?query=workflow%3AKaisa+branch%3Amain)
[![NuGet Version](https://img.shields.io/nuget/v/Kaisa?style=flat-square)](https://www.nuget.org/packages/Kaisa/)
[![Sponsor](https://img.shields.io/badge/sponsor-%E2%9D%A4-lightgrey?logo=github&style=flat-square)](https://github.com/sponsors/PathogenDavid)

Kaisa is a C# parser for various library files consumed by linkers. It is primarily intended to support locating where and how symbols are exported by dynamic and static libraries. However basic information about unrelated object sections is exposed for advanced users.

The following formats are currently supported:

* [Windows `.lib` archive/library files](https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#archive-library-file-format)
* [Linux `.a` archive files](https://refspecs.linuxfoundation.org/elf/gabi41.pdf#page=152)
* [Linux `.o`/`.so` ELF files](https://refspecs.linuxfoundation.org/elf/gabi4+/contents.html)

It also supports automatic identification of such files. Including a best guess differentiation between Windows and Linux archives (which use the same-yet-different non-standardized format.)

Note that PE images (IE: `.dll` files) are not supported since linkers on Windows do not consume them.

## License

This project is licensed under the MIT License. [See the license file for details](LICENSE.txt).

## Archive file feature support

The parser should tolerate any library archive file, but only the following information is explicitly parsed and exposed:

* [V1 index files](https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#first-linker-member) (AKA "first linker member")
* [V2 index files*](https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#second-linker-member) (AKA "second linker member")
* [The longnames file](https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#longnames-member)
* [Import members*](https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#import-library-format)
* COFF object members*
  * [Header](https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#coff-file-header-object-and-image) (all members)
  * [The section table](https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#section-table-section-headers)
    * Extended names from the string table (IE: `/123`) are currently not resolved
  * [The symbol table](https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#coff-symbol-table), including [the string table](https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#coff-string-table).
    * Note that the string table is not exposed on the API surface. It is discarded after the symbol table is read.
* ELF object members** (see ELF support below for details.)
* Unrecognized members (only exposes a data range in case you want to parse something exotic yourself)
  * "Unrecognized" is tricky due to COFF members having no magic signature. For a member to be considered unrecognized, all of the following must be true:
    * It is not recognized as an import object
    * It is not recognized as an ELF file
    * It would have an invalid [COFF machine type](https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#machine-types) if it were parsed as a COFF object

<sup>\* Specific to Windows archives<br>\*\* Specific to Linux archives</sup>

Note that the parser currently requires the symbol index file to be present. As such it cannot be used to parse non-library archive files such as `.deb` packages. It also does not currently understand BSD-style library archives (including those used on macOS.)

### Unsupported COFF object member features

The following are explicitly not parsed or exposed for COFF objects: (Although enough information is exposed that you should be able to parse them yourself.)

* The optional (image-only) COFF header
* Section data
* COFF relocations
* COFF line numbers
* The (image-only) attribute certificate table
* Delay-load import only tables (image-only)

### Other unsupported features

Some AR archive fields that are meaningless to the Microsoft spec (such as the UID and GID) are skipped when parsing. (They aren't particularly useful or important to linkers on Linux either, they're an artifact from archive files being a general-purpose archive format.)

## ELF file feature support

The parser should tolerate any ELF file, but only the following sections are explicitly parsed and exposed:

* [General and dynamic symbols tables](https://refspecs.linuxfoundation.org/elf/gabi4+/ch4.symtab.html) (`SHT_SYMTAB` and `SHT_DYNSYM`)
  * Extended index tables (`SHT_SYMTAB_SHNDX`) -- Not directly exposed but extended indices are automatically resolved on symbols.
* [String tables](https://refspecs.linuxfoundation.org/elf/gabi4+/ch4.strtab.html) (`SHT_STRTAB`)
* Basic metadata of other unrecognized sections
  * Including whether the section is a [special well-known section](https://refspecs.linuxfoundation.org/elf/gabi4+/ch4.sheader.html#special_sections) or a non-standard operating system or processor-specific extension section.
  * Advanced users can extract the range of these sections for manual parsing if so desired.

Additionally, relevant [x64-specific extensions](https://refspecs.linuxfoundation.org/elf/x86_64-abi-0.99.pdf#page=61) are exposed. (Although not many actually affect the features we do expose.)

In theory 32-bit ELF files are supported, but their support is not regularly exercised. Big endian ELF files are not supported unless your app is running on a big endian platform. Developers with more advanced needs might consider evaluating [LibObjectFile](https://github.com/xoofx/LibObjectFile) instead.

### Unsupported ELF features

[The program headers](https://refspecs.linuxfoundation.org/elf/gabi4+/ch5.pheader.html) (if present) are not parsed. However, their offset and size is exposed via `ElfFile.Header` for advanced users who wish to parse them manually.

## API Stability

This library primarily exists to support the librarian functionality in [Biohazrd](https://github.com/InfectedLibraries/Biohazrd). Not a ton of thought went into the API, especially for things not needed by Biohazrd. As such I might tweak the API to better support these needs as they evolve. I consider this library to be pretty niche, so if you use this library in a major way consider posting a discussion or [DMing me](https://twitter.com/pathogendavid) so I know to avoid breaking you.

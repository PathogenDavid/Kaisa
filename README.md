Kaisa the Sharp Librarian
===============================================================================

[![MIT Licensed](https://img.shields.io/github/license/pathogendavid/kaisa?style=flat-square)](LICENSE.txt)
[![CI Status](https://img.shields.io/github/workflow/status/pathogendavid/kaisa/Kaisa/main?style=flat-square)](https://github.com/PathogenDavid/Kaisa/actions?query=workflow%3AKaisa+branch%3Amain)
[![Sponsor](https://img.shields.io/badge/sponsor-%E2%9D%A4-lightgrey?logo=github&style=flat-square)](https://github.com/sponsors/PathogenDavid)

Kaisa is a C# parser for [Windows `.lib` archive/library files](https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#archive-library-file-format). It is primarily intended to read import libraries, but it does provide basic support for parsing object files too.

## License

This project is licensed under the MIT License. [See the license file for details](LICENSE.txt).

## Library file feature support

The parser should tolerate any library file, but only the following information is explicitly parsed and exposed:

* [V1 index files](https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#first-linker-member) (AKA "first linker member")
* [V2 index files](https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#second-linker-member) (AKA "second linker member")
* [The longnames file](https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#longnames-member)
* [Import objects](https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#import-library-format)
* COFF objects
  * [Header](https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#coff-file-header-object-and-image) (all members)
    * Extended names from the string table (IE: `/123`) are not resolved
  * [The section table](https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#section-table-section-headers)
  * [The symbol table](https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#coff-symbol-table), including [the string table](https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#coff-string-table).
    * Note that the string table is not exposed on the API surface. It is discarded after the symbol table is read.

The following are explicitly not parsed or exposed for COFF objects: (Although enough information is exposed that you should be able to parse them yourself.)

* The optional (image-only) COFF header
* Section data
* COFF relocations
* COFF line numbers
* The (image-only) attribute certificate table
* Delay-load import only tables (image-only)

There is currently no attempt at parsing System V or GNU AR archives since it is currently implemented from the Microsoft spec. Some AR archive fields that are meaningless to the Microsoft spec (such as the UID and GID) are skipped when parsing.

There is no support for PE (image) files. (You can probably use [System.Reflection.PortableExecutable](https://docs.microsoft.com/en-us/dotnet/api/system.reflection.portableexecutable) for those.)

## API Stability

This library primarily exists to support the librarian functionality in [Biohazrd](https://github.com/InfectedLibraries/Biohazrd). Not a ton of thought went into the API, especially for things not needed by Biohazrd. As such I might tweak the API to better support these needs as they evolve. I consider this library to be pretty niche, so if you use this library in a major way consider posting a discussion or [DMing me](https://twitter.com/pathogendavid) so I know to avoid breaking you.

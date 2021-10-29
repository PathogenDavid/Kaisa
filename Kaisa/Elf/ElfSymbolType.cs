namespace Kaisa.Elf
{
    public enum ElfSymbolType : byte
    {
        /// <summary>The symbol's type is not specified.</summary>
        None = 0,
        /// <summary>The symbol is associated with a data object, such as a variable, an array, and so on.</summary>
        Object = 1,
        /// <summary>The symbol is associated with a function or other executable code.</summary>
        Function = 2,
        /// <summary>The symbol is associated with a section. Symbol table entries of this type exist primarily for relocation and normally have <see cref="ElfSymbolBinding.Local"/> binding.</summary>
        Section = 3,
        /// <summary>Conventionally, the symbol's name gives the name of the source file associated with the object file.</summary>
        File = 4,
        /// <summary>The symbol labels an uninitialized common block.</summary>
        Common = 5,
        /// <summary>The symbol specifies a Thread-Local Storage entity.</summary>
        ThreadLocalStorage = 6,

        FirstOperatingSystemSpecific = 10,
        LastOperatingSystemSpecific = 12,
        FirstProcessorSpecific = 13,
        LastOperatorSpecific = 15,
    }
}

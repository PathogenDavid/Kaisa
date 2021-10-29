namespace Kaisa.Elf
{
    public enum ElfSymbolVisibility : byte
    {
        /// <summary>
        /// The default visibility based on <see cref="ElfSymbol.Binding"/>.
        /// <see cref="ElfSymbolBinding.Local"/> symbols are hidden,
        /// <see cref="ElfSymbolBinding.Global"/> and <see cref="ElfSymbolBinding.Weak"/> are externally visible and preemptable.
        /// </summary>
        /// <remarks>An implementation is permitted to restrict global and weak symbols beyond the default.</remarks>
        Default = 0,
        /// <summary>Processor-specific visibility, treat as <see cref="Hidden"/> if not understood.</summary>
        Internal = 1,
        /// <summary>The symbol is not externally visible and is not preemptable.</summary>
        Hidden = 2,
        /// <summary>Indicates that the symbol is externally visible but is not preemptable. May not be used with <see cref="ElfSymbolBinding.Local"/> symbols.</summary>
        Protected = 3,
    }
}

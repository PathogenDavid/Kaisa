namespace Kaisa
{
    // https://docs.microsoft.com/en-us/windows/win32/debug/pe-format#type-representation
    public enum CoffSymbolComplexType : byte
    {
        /// <summary>No derived type; the symbol is a simple scalar variable.</summary>
        Null = 0,
        /// <summary>The symbol is a pointer to base type.</summary>
        Pointer = 1,
        /// <summary>The symbol is a function that returns a base type.</summary>
        Function = 2,
        /// <summary>The symbol is an array of base type.</summary>
        Array = 3,
    }
}

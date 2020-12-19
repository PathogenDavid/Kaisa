using System;
using System.Runtime.CompilerServices;

namespace Kaisa
{
    public unsafe static class SpanExtensions
    {
        public static Span<T> Single<T>(ref T element)
            where T : unmanaged
            => new Span<T>(Unsafe.AsPointer(ref element), 1);

        public static ReadOnlySpan<T> SingleReadOnly<T>(in T element)
            where T : unmanaged
            => new Span<T>(Unsafe.AsPointer(ref Unsafe.AsRef(element)), 1);
    }
}

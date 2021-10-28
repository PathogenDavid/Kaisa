using System;

namespace Kaisa
{
    public sealed class MalformedFileException : Exception
    {
        /// <summary>The offset in the input stream where the problem was detected.</summary>
        public long ProblematicOffset { get; }

        public MalformedFileException(string message, long problematicOffset)
            : this(message, problematicOffset, innerException: null)
        { }

        public MalformedFileException(string message, long problematicOffset, Exception? innerException)
            : base($"Problem at offset {problematicOffset}: {message}", innerException)
            => ProblematicOffset = problematicOffset;
    }
}

using System;
using System.Diagnostics;
using System.IO;

namespace Kaisa
{
    /// <summary>A wrapper of a <see cref="Stream"/> that validates reads only occur within the specified span.</summary>
    public sealed class ScopedStream : Stream
    {
        private readonly Stream WrappedStream;
        private readonly long SpanStart;
        private readonly long SpanLength;
        private long SpanEnd => SpanStart + SpanLength;

        public override bool CanRead => WrappedStream.CanRead;
        public override bool CanSeek => WrappedStream.CanSeek;
        public override bool CanWrite => false;
        public override long Length => WrappedStream.Length;

        public override long Position
        {
            get => WrappedStream.Position;
            set => Seek(value, SeekOrigin.Begin);
        }

        public ScopedStream(Stream stream, long spanStart, long spanLength)
        {
            WrappedStream = stream;
            SpanStart = spanStart;
            SpanLength = spanLength;

            if (WrappedStream.Position < SpanStart)
            { throw new InvalidOperationException("The stream would be starting before the specified span!"); }
            else if (WrappedStream.Position > SpanEnd)
            { throw new InvalidOperationException("The stream would be starting after the specified span!"); }
        }

        public override int Read(byte[] buffer, int offset, int count)
            => Read(buffer.AsSpan().Slice(offset, count));

        public override int Read(Span<byte> buffer)
        {
            if (Position < SpanStart)
            {
                Debug.Fail("Read attempted before permitted span.");
                return 0;
            }
            else if (Position >= SpanEnd)
            {
                Debug.Fail("Read attempted after permitted span.");
                return 0;
            }

            // Truncate the read if it exceeds the allowed span
            long endPosition = Position + buffer.Length;
            if (endPosition > SpanEnd)
            {
                Debug.Fail("Read attempted which exceeded the permitted span.");
                buffer = buffer.Slice(0, buffer.Length - checked((int)(endPosition - SpanEnd)));
            }

            int result = WrappedStream.Read(buffer);
            Debug.Assert(Position <= SpanEnd); // Inclusive since cursor can be at the very end
            return result;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            long newPosition = origin switch
            {
                SeekOrigin.Begin => offset,
                SeekOrigin.Current => WrappedStream.Position + offset,
                SeekOrigin.End => WrappedStream.Length - offset,
                _ => throw new ArgumentException("The seek origin is invalid.", nameof(origin))
            };

            // Stream.Seek normally allows seeking outside of the stream, but we don't care about supporting this and want to catch invalid seeks.
            // Note that the end is exclusive so the stream cursor can sit on the end of the stream.
            if (newPosition < SpanStart || newPosition > SpanStart + SpanLength)
            { throw new IOException("Seek landed outside of the permitted region."); }

            return WrappedStream.Position = newPosition;
        }

        public override void Flush()
            => throw new NotSupportedException();

        public override void SetLength(long value)
            => throw new NotSupportedException();

        public override void Write(byte[] buffer, int offset, int count)
            => throw new NotSupportedException();
    }
}

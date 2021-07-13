namespace KafkaFlow.Client.Protocol.Streams
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;

    public abstract class BaseMemoryStream : Stream, IReadOnlyList<byte>
    {
        protected long length = 0;

        ~BaseMemoryStream()
        {
            this.Dispose(false);
        }

        public override void Flush()
        {
            // Do nothing
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int Read(byte[] buffer, int offset, int count) => this.Read(new Span<byte>(buffer, offset, count));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Write(byte[] buffer, int offset, int count) => this.Write(new Span<byte>(buffer, offset, count));

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    return this.Position = offset;

                case SeekOrigin.Current:
                    return this.Position += offset;

                case SeekOrigin.End:
                    return this.Position = this.Length + offset;

                default:
                    throw new ArgumentOutOfRangeException(nameof(origin), origin, null);
            }
        }

        public abstract Span<byte> GetSpan(int size);

        public abstract byte this[int index] { get; }

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => true;

        public override long Length => this.length;

        public IEnumerator<byte> GetEnumerator() => new BaseMemoryStreamEnumerator(this);

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public int Count => (int) this.length;

        private class BaseMemoryStreamEnumerator : IEnumerator<byte>
        {
            private int position = -1;
            private readonly BaseMemoryStream stream;

            public BaseMemoryStreamEnumerator(BaseMemoryStream stream)
            {
                this.stream = stream;
            }

            public bool MoveNext() => ++this.position < this.stream.Length;

            public void Reset() => this.position = -1;

            public byte Current => this.position < this.stream.Length ? this.stream[this.position] : default;

            object IEnumerator.Current => this.Current;

            public void Dispose()
            {
                // Dd nothing
            }
        }
    }
}

namespace KafkaFlow.Client.Protocol.Streams
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;

    public class TrackedStream : BaseMemoryStream
    {
        private readonly BaseMemoryStream stream;
        private long position;
        private long length = 0;
        private readonly long offsetPosition;

        public TrackedStream(BaseMemoryStream stream, int size)
        {
            this.Size = size;
            this.stream = stream;

            if (stream.CanSeek)
            {
                this.offsetPosition = stream.Position;
            }
        }

        public int Size { get; }

        public override bool CanRead => this.stream.CanRead;

        public override bool CanSeek => this.stream.CanSeek;

        public override bool CanWrite => this.stream.CanWrite;

        public override long Length => this.length;

        public override long Position
        {
            get => this.position;
            set
            {
                if (!this.stream.CanSeek)
                {
                    throw new NotSupportedException();
                }

                this.position = value;
                this.stream.Position = this.offsetPosition + value;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void MovePosition(long offset)
        {
            if ((this.position += offset) > this.Size)
                throw new ArgumentOutOfRangeException(nameof(this.Position), "The stream size can't be exceeded");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Flush() => this.stream.Flush();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int Read(byte[] buffer, int offset, int count)
        {
            this.MovePosition(count);
            return this.stream.Read(buffer, offset, count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int Read(Span<byte> buffer)
        {
            this.MovePosition(buffer.Length);
            return this.stream.Read(buffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int ReadByte()
        {
            this.MovePosition(sizeof(byte));
            return this.stream.ReadByte();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Write(ReadOnlySpan<byte> buffer)
        {
            this.MovePosition(buffer.Length);
            this.stream.Write(buffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void WriteByte(byte value)
        {
            this.MovePosition(sizeof(byte));
            this.stream.WriteByte(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override long Seek(long offset, SeekOrigin origin)
        {
            this.MovePosition(offset);
            return this.stream.Seek(offset, origin);
        }

        public override Span<byte> GetSpan(int size)
        {
            throw new NotImplementedException();
        }

        public override byte this[int index] => this.stream[index];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void SetLength(long value) => throw new NotSupportedException();


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Write(byte[] buffer, int offset, int count)
        {
            this.MovePosition(count);
            this.stream.Write(buffer, offset, count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DiscardRemainingData()
        {
            var remaining = (int) (this.Size - this.Position);

            if (remaining == 0)
                return;

            Span<byte> buffer = stackalloc byte[1024];

            do
            {
                var read = this.stream.Read(buffer.Slice(0, Math.Min(remaining, buffer.Length)));

                if (read == 0)
                    return;

                remaining -= read;
            } while (remaining > 0);
        }
    }
}

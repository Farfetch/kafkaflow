namespace KafkaFlow.Client.Protocol
{
    using System;
    using System.IO;
    using System.Runtime.CompilerServices;

    public class TrackedStream : Stream
    {
        private readonly Stream stream;
        private long position;

        public TrackedStream(Stream stream, int size)
        {
            this.Size = size;
            this.stream = stream;
        }

        public int Size { get; }

        public override bool CanRead => this.stream.CanRead;

        public override bool CanSeek => this.stream.CanSeek;

        public override bool CanWrite => this.stream.CanWrite;

        public override long Length => this.stream.Length;

        public override long Position
        {
            get => this.position;
            set => throw new NotSupportedException();
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
        public override long Seek(long offset, SeekOrigin origin)
        {
            this.MovePosition(offset);
            return this.stream.Seek(offset, origin);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void SetLength(long value)
        {
            this.stream.SetLength(value);
        }

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

            using var buffer = new PooledArray<byte>(1024);

            do
            {
                remaining -= this.stream.Read(buffer, 0, Math.Min(remaining, buffer.Length));
            } while (remaining > 0);
        }
    }
}

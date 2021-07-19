namespace KafkaFlow.Client.Protocol.Streams
{
    using System;
    using System.Buffers;
    using System.IO;
    using System.Runtime.CompilerServices;

    public sealed class MemoryReader : IDisposable
    {
        private readonly byte[] buffer;
        private int position;

        public MemoryReader(int length)
        {
            this.buffer = ArrayPool<byte>.Shared.Rent(length);
            this.Length = length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ReadFrom(Stream stream)
        {
            stream.Read(new Span<byte>(this.buffer, this.position, this.Length - this.position));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> GetSpan(int size)
        {
            var start = this.position;
            this.Position += size;
            return new Span<byte>(this.buffer, start, size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte ReadByte() => this.buffer[this.Position++];

        public void Dispose() => ArrayPool<byte>.Shared.Return(this.buffer);

        public int Length { get; }

        public int Position
        {
            get => this.position;
            set
            {
                if (value < 0 || value > this.Length)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(this.Position),
                        $"The {nameof(this.Position)} must be greater or equal 0");
                }

                this.position = value;
            }
        }
    }
}

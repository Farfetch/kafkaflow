namespace KafkaFlow.Client.Protocol.Streams
{
    using System;
    using System.Buffers;
    using System.IO;
    using System.Runtime.CompilerServices;

    internal sealed class MemoryReader : IDisposable
    {
        private readonly byte[] buffer;
        private int position;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryReader"/> class.
        /// </summary>
        /// <param name="length">The memory reader length</param>
        public MemoryReader(int length)
        {
            this.buffer = ArrayPool<byte>.Shared.Rent(length);
            this.Length = length;
        }

        /// <summary>
        /// Gets the length memory reader length
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// Gets or sets the current position of the memory writer
        /// </summary>
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

        /// <inheritdoc/>
        public void Dispose() => ArrayPool<byte>.Shared.Return(this.buffer);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void ReadFrom(Stream stream)
        {
            stream.Read(new Span<byte>(this.buffer, this.position, this.Length - this.position));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Span<byte> GetSpan(int size)
        {
            var start = this.position;
            this.Position += size;
            return new Span<byte>(this.buffer, start, size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal byte ReadByte() => this.buffer[this.Position++];
    }
}

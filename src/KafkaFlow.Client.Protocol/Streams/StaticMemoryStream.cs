namespace KafkaFlow.Client.Protocol.Streams
{
    using System;
    using System.Buffers.Binary;
    using System.IO;
    using System.Runtime.CompilerServices;

    public sealed class StaticMemoryStream : BaseMemoryStream
    {
        private readonly IntPtr buffer;
        private readonly IMemoryManager memoryManager;
        private long position;

        public StaticMemoryStream(IMemoryManager memoryManager, int length)
        {
            this.memoryManager = memoryManager;
            this.buffer = memoryManager.Allocate(length);
            this.length = length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override unsafe int Read(Span<byte> buffer)
        {
            var writeCount = (int) Math.Min(this.length - this.position, buffer.Length);

            new Span<byte>((this.buffer + (int) this.position).ToPointer(), writeCount)
                .CopyTo(buffer);

            this.position += writeCount;

            return writeCount;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe void ReadFrom(Stream stream)
        {
            stream.Read(
                new Span<byte>(
                    (this.buffer + (int) this.position).ToPointer(),
                    (int) (this.length - this.position)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe int ReadInt32Native()
        {
            this.position += sizeof(int);
            return BinaryPrimitives.ReadInt32BigEndian(
                new Span<byte>(
                    (void*) (this.buffer + (int) this.position - sizeof(int)),
                    sizeof(int)));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override unsafe Span<byte> GetSpan(int size)
        {
            this.Position += sizeof(int);
            return new Span<byte>(this.buffer.ToPointer(), size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override unsafe void Write(ReadOnlySpan<byte> buffer)
        {
            var writeCount = (int) Math.Min(this.length - this.position, buffer.Length);

            buffer.CopyTo(new Span<byte>((this.buffer + (int) this.position).ToPointer(), writeCount));

            this.position += writeCount;
        }

        public override void SetLength(long value) => throw new NotSupportedException();

        protected override void Dispose(bool disposing) => this.memoryManager.Free(this.buffer);

        public override unsafe byte this[int index] => *(byte*) (this.buffer + (int) this.position);

        public override long Position
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

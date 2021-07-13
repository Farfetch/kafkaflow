namespace KafkaFlow.Client.Protocol.Streams
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.IO;
    using System.Net.Sockets;
    using System.Runtime.CompilerServices;

    public sealed class DynamicMemoryStream : BaseMemoryStream
    {
        private int currentSegment = 0;
        private int relativePosition = 0;

        private readonly IMemoryManager memoryManager;
        private readonly int segmentSize;

        private readonly List<IntPtr> segments = new();

        public DynamicMemoryStream(IMemoryManager memoryManager, int segmentSize)
        {
            this.memoryManager = memoryManager;
            this.segmentSize = segmentSize;
        }

        public DynamicMemoryStream(IMemoryManager memoryManager) : this(memoryManager, 1024)
        {
        }

        public override unsafe int Read(Span<byte> buffer)
        {
            var offset = 0;
            var count = buffer.Length;
            var startSegment = this.currentSegment;
            var startRelPosition = this.relativePosition;

            var startPosition = this.Position;

            count = (int) Math.Min(this.Length - startPosition, count);
            var countToReturn = count;

            this.Position = startPosition + count;

            while (count > 0)
            {
                var segment = this.segments[startSegment++];

                var writeCount = Math.Min(this.segmentSize - startRelPosition, count);

                new Span<byte>((segment + startRelPosition).ToPointer(), writeCount)
                    .CopyTo(buffer.Slice(offset, writeCount));

                startRelPosition = 0;

                offset += writeCount;
                count -= writeCount;
            }

            return countToReturn;
        }

        public unsafe void ReadFrom(NetworkStream stream, int count)
        {
            var endPosition = this.Position + count;

            this.EnsureCapacity(endPosition);

            var startRelPosition = this.relativePosition;
            var startSegment = this.currentSegment;

            this.Position = endPosition;

            while (count > 0)
            {
                var segment = this.segments[startSegment++];

                var writeCount = Math.Min(this.segmentSize - startRelPosition, count);

                stream.Read(new Span<byte>((segment + startRelPosition).ToPointer(), writeCount));

                startRelPosition = 0;
                count -= writeCount;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override void Write(byte[] buffer, int offset, int count) => this.Write(new Span<byte>(buffer, offset, count));

        public override unsafe void Write(ReadOnlySpan<byte> buffer)
        {
            var count = buffer.Length;
            var offset = 0;
            var endPosition = this.Position + count;

            this.EnsureCapacity(endPosition);

            var startRelPosition = this.relativePosition;
            var startSegment = this.currentSegment;

            this.Position = endPosition;

            while (count > 0)
            {
                var segment = this.segments[startSegment++];

                var writeCount = Math.Min(this.segmentSize - startRelPosition, count);

                buffer
                    .Slice(offset, writeCount)
                    .CopyTo(new Span<byte>((segment + startRelPosition).ToPointer(), writeCount));

                startRelPosition = 0;

                offset += writeCount;
                count -= writeCount;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public new void CopyTo(Stream destination)
        {
            if (destination is DynamicMemoryStream fast)
            {
                this.FastCopyTo(fast);
            }
            else
            {
                this.InternalCopyTo(destination);
            }
        }

        private unsafe void InternalCopyTo(Stream destination)
        {
            var totalBytes = this.length;

            foreach (var segment in this.segments)
            {
                var bytesToWriteCount = (int) Math.Min(totalBytes, this.segmentSize);
                destination.Write(new ReadOnlySpan<byte>(segment.ToPointer(), bytesToWriteCount));

                totalBytes -= bytesToWriteCount;
            }
        }

        private unsafe void FastCopyTo(DynamicMemoryStream dest)
        {
            var sourceSegmentIndex = this.currentSegment;
            var sourceOffset = this.relativePosition;

            var destSegmentIndex = dest.currentSegment;
            var destOffset = dest.relativePosition;

            var totalWriteCount = this.Length - this.Position;

            dest.EnsureCapacity(dest.Position + totalWriteCount);

            this.Position = this.Length;
            dest.Position += totalWriteCount;

            while (totalWriteCount > 0)
            {
                var sourceSegment = this.segments[sourceSegmentIndex];
                var destSegment = dest.segments[destSegmentIndex];

                var sourceWriteCount = (int) Math.Min(this.segmentSize - sourceOffset, totalWriteCount);
                var destWriteCount = (int) Math.Min(dest.segmentSize - destOffset, totalWriteCount);
                var writeCount = Math.Min(sourceWriteCount, destWriteCount);

                Buffer.MemoryCopy(
                    (sourceSegment + sourceOffset).ToPointer(),
                    (destSegment + destOffset).ToPointer(),
                    writeCount,
                    writeCount);

                totalWriteCount -= writeCount;

                if (dest.segmentSize == writeCount + destOffset)
                {
                    destOffset = 0;
                    ++destSegmentIndex;
                }
                else
                {
                    destOffset += writeCount;
                }

                if (this.segmentSize == writeCount + sourceOffset)
                {
                    sourceOffset = 0;
                    ++sourceSegmentIndex;
                }
                else
                {
                    sourceOffset += writeCount;
                }
            }
        }

        public override void SetLength(long value)
        {
            this.EnsureCapacity(value);
            this.length = value;
        }

        protected override void Dispose(bool disposing)
        {
            foreach (var segment in this.segments)
            {
                this.memoryManager.Free(segment);
            }

            this.segments.Clear();
        }

        private void EnsureCapacity(long capacity)
        {
            if (capacity <= this.Capacity)
                return;

            var diff = capacity - this.Capacity;

            var newSegmentsCount = (int) Math.Ceiling(diff / (double) this.segmentSize);
            this.Capacity += newSegmentsCount * this.segmentSize;

            while (--newSegmentsCount >= 0)
            {
                this.segments.Add(this.memoryManager.Allocate(this.segmentSize));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetSegment(long globalPosition) => (int) Math.Floor(globalPosition / (double) this.segmentSize);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetRelativePosition(long globalPosition) => (int) (globalPosition % this.segmentSize);

        public override unsafe byte this[int index] => *(byte*) (this.segments[this.GetSegment(index)] + this.GetRelativePosition(index));

        public override long Position
        {
            get => this.segmentSize * this.currentSegment + this.relativePosition;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(this.Position),
                        $"The {nameof(this.Position)} must be greater or equal 0");
                }

                if (value > this.length)
                    this.length = value;

                this.currentSegment = this.GetSegment(value);
                this.relativePosition = this.GetRelativePosition(value);
            }
        }

        public long Capacity { get; private set; }
    }
}

namespace KafkaFlow.Client.Protocol.Streams
{
    using System;
    using System.Buffers;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;

    internal sealed class MemoryWriter : IDisposable
    {
        private readonly int segmentSize;
        private readonly List<byte[]> segments = new();
        private int currentSegment;
        private int relativePosition;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryWriter"/> class.
        /// </summary>
        /// <param name="segmentSize">The size of the segment</param>
        public MemoryWriter(int segmentSize)
        {
            this.segmentSize = segmentSize;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryWriter"/> class.
        /// </summary>
        public MemoryWriter()
            : this(1024 * 8)
        {
        }

        /// <summary>
        /// Gets or sets the current position of the memory writer
        /// </summary>
        public int Position
        {
            get => (this.segmentSize * this.currentSegment) + this.relativePosition;
            set
            {
                if (value < 0 && value <= this.Capacity)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(this.Position),
                        $"The {nameof(this.Position)} must be greater or equal 0 and less than Capacity");
                }

                if (value > this.Length)
                {
                    this.Length = value;
                }

                this.currentSegment = this.GetSegment(value);
                this.relativePosition = this.GetRelativePosition(value);
            }
        }

        internal int Length { get; private set; }

        private int Capacity { get; set; }

        /// <inheritdoc/>
        public void Dispose()
        {
            foreach (var segment in this.segments)
            {
                ArrayPool<byte>.Shared.Return(segment);
            }

            this.segments.Clear();
        }

        internal void Write(ReadOnlySpan<byte> buffer)
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
                    .CopyTo(new Span<byte>(segment, startRelPosition, writeCount));

                startRelPosition = 0;

                offset += writeCount;
                count -= writeCount;
            }
        }

        internal bool TryGetSpan(int length, out Span<byte> span)
        {
            if (this.segments.Count == 0 || this.segmentSize - this.relativePosition <= length)
            {
                span = Span<byte>.Empty;
                return false;
            }

            span = new Span<byte>(this.segments[this.currentSegment], this.relativePosition, length);

            return true;
        }

        internal void Advance(int length) => this.Position += length;

        internal void CopyTo(Stream destination)
        {
            var totalBytes = this.Length;

            foreach (var segment in this.segments)
            {
                var bytesToWriteCount = Math.Min(totalBytes, this.segmentSize);
                destination.Write(new ReadOnlySpan<byte>(segment, 0, bytesToWriteCount));

                totalBytes -= bytesToWriteCount;
            }
        }

        internal byte this[int index] => this.segments[this.GetSegment(index)][this.GetRelativePosition(index)];

        internal void CopyTo(MemoryWriter dest)
        {
            var sourceSegmentIndex = this.currentSegment;
            var sourceOffset = this.relativePosition;

            var destSegmentIndex = dest.currentSegment;
            var destOffset = dest.relativePosition;

            var totalWriteCount = this.Length - this.Position;

            dest.EnsureCapacity(dest.Position + totalWriteCount);

            this.Position = this.Length;
            dest.Position += totalWriteCount;

            if (dest.Position > dest.Length)
            {
                dest.Length = dest.Position;
            }

            while (totalWriteCount > 0)
            {
                var sourceSegment = this.segments[sourceSegmentIndex];
                var destSegment = dest.segments[destSegmentIndex];

                var sourceWriteCount = Math.Min(this.segmentSize - sourceOffset, totalWriteCount);
                var destWriteCount = Math.Min(dest.segmentSize - destOffset, totalWriteCount);
                var writeCount = Math.Min(sourceWriteCount, destWriteCount);

                new Span<byte>(sourceSegment, sourceOffset, writeCount)
                    .CopyTo(new Span<byte>(destSegment, destOffset, writeCount));

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

        private void EnsureCapacity(long capacity)
        {
            if (capacity <= this.Capacity)
            {
                return;
            }

            var diff = capacity - this.Capacity;

            var newSegmentsCount = (int)Math.Ceiling(diff / (double)this.segmentSize);
            this.Capacity += newSegmentsCount * this.segmentSize;

            while (--newSegmentsCount >= 0)
            {
                this.segments.Add(ArrayPool<byte>.Shared.Rent(this.segmentSize));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetSegment(long globalPosition) => (int)Math.Floor(globalPosition / (double)this.segmentSize);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetRelativePosition(long globalPosition) => (int)(globalPosition % this.segmentSize);
    }
}

namespace KafkaFlow.Client.Protocol
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public class FastMemoryStream : Stream, IReadOnlyList<byte>
    {
        private long length = 0;
        private int currentSegment = 0;
        private int relativePosition = 0;

        private readonly int segmentSize;

        private readonly List<IntPtr> segments = new List<IntPtr>();

        public FastMemoryStream(int segmentSize)
        {
            this.segmentSize = segmentSize;
        }

        public FastMemoryStream() : this(1024)
        {
        }

        ~FastMemoryStream()
        {
            this.Dispose(false);
        }

        public override void Flush()
        {
            // Do nothing
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
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

                Marshal.Copy(segment + startRelPosition, buffer, offset, writeCount);

                startRelPosition = 0;

                offset += writeCount;
                count -= writeCount;
            }

            return countToReturn;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            var startPosition = this.Position;
            var endPosition = startPosition + count;

            this.EnsureCapacity(endPosition);

            var startRelPosition = this.relativePosition;
            var startSegment = this.currentSegment;

            this.Position = endPosition;

            while (count > 0)
            {
                var segment = this.segments[startSegment++];

                var writeCount = Math.Min(this.segmentSize - startRelPosition, count);

                Marshal.Copy(buffer, offset, segment + startRelPosition, writeCount);

                startRelPosition = 0;

                this.length += writeCount;
                offset += writeCount;
                count -= writeCount;
            }
        }

        public new void CopyTo(Stream destination)
        {
            if (destination is FastMemoryStream fast)
                this.FastCopyTo(fast);
            else
                base.CopyTo(destination);
        }

        private unsafe void FastCopyTo(FastMemoryStream dest)
        {
            var sourceSegmentIndex = this.currentSegment;
            var sourceOffset = this.relativePosition;

            var destSegmentIndex = dest.currentSegment;
            var destOffset = dest.relativePosition;

            var totalWriteCount = this.Length - this.Position;

            dest.EnsureCapacity(dest.Length + totalWriteCount);

            this.Position = this.Length;
            dest.Position += totalWriteCount;

            if (dest.Position > dest.Length)
                dest.length = dest.Position;

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

        public override void SetLength(long value)
        {
            this.EnsureCapacity(value);
            this.length = value;
        }

        public byte[] ToArray()
        {
            var result = new byte[this.Length];

            var position = this.Position;
            this.Position = 0;

            this.Read(result);

            this.Position = position;

            return result;
        }

        protected override void Dispose(bool disposing)
        {
            foreach (var segment in this.segments)
            {
                Marshal.FreeHGlobal(segment);
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
                this.segments.Add(Marshal.AllocHGlobal(this.segmentSize));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetSegment(long globalPosition) => (int) Math.Floor(globalPosition / (double) this.segmentSize);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetRelativePosition(long globalPosition) => (int) (globalPosition % this.segmentSize);

        public byte this[int index] => Marshal.ReadByte(this.segments[this.GetSegment(index)] + this.GetRelativePosition(index));

        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => true;

        public override long Length => this.length;

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

                this.currentSegment = this.GetSegment(value);
                this.relativePosition = this.GetRelativePosition(value);
            }
        }

        public long Capacity { get; private set; }

        public IEnumerator<byte> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public int Count => (int) this.length;
    }
}

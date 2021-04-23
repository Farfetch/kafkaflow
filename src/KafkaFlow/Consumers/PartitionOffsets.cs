namespace KafkaFlow.Consumers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;

    internal class PartitionOffsets
    {
        private readonly SortedSet<long> pendingOffsets = new();
        private readonly LinkedList<long> offsetsOrder = new();

        public long LastOffset { get; private set; } = -1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddOffset(long offset)
        {
            lock (this.offsetsOrder)
            {
                this.offsetsOrder.AddLast(offset);
            }
        }

        public bool ShouldUpdateOffset(long newOffset)
        {
            lock (this.offsetsOrder)
            {
                if (!this.offsetsOrder.Any())
                {
                    throw new InvalidOperationException(
                        $"There is no offsets in the queue. Call {nameof(this.AddOffset)} first");
                }

                if (newOffset != this.offsetsOrder.First.Value)
                {
                    this.pendingOffsets.Add(newOffset);
                    return false;
                }

                do
                {
                    this.LastOffset = this.offsetsOrder.First.Value;
                    this.offsetsOrder.RemoveFirst();
                }
                while (this.offsetsOrder.Count > 0 && this.pendingOffsets.Remove(this.offsetsOrder.First.Value));
            }

            return true;
        }
    }
}

namespace KafkaFlow.Consumers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class PartitionOffsets
    {
        private readonly SortedSet<long> processedOffsets = new();
        private readonly LinkedList<long> receivedOffsets = new();

        public void Enqueue(long offset)
        {
            lock (this.receivedOffsets)
            {
                this.receivedOffsets.AddLast(offset);
            }
        }

        public bool ShouldCommit(long offset, out long lastProcessedOffset)
        {
            lastProcessedOffset = -1;
            lock (this.receivedOffsets)
            {
                if (!this.receivedOffsets.Any())
                {
                    throw new InvalidOperationException(
                        $"There is no offsets in the received queue. Call {nameof(this.Enqueue)} first");
                }

                if (offset != this.receivedOffsets.First.Value)
                {
                    this.processedOffsets.Add(offset);
                    return false;
                }

                do
                {
                    lastProcessedOffset = this.receivedOffsets.First.Value;
                    this.receivedOffsets.RemoveFirst();
                }
                while (this.receivedOffsets.Count > 0 && this.processedOffsets.Remove(this.receivedOffsets.First.Value));
            }

            return true;
        }
    }
}

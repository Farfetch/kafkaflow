namespace KafkaFlow.Consumers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    internal class PartitionOffsets
    {
        private readonly SortedDictionary<long, IConsumerContext> processedContexts = new();
        private readonly LinkedList<IConsumerContext> receivedContexts = new();

        public void Enqueue(IConsumerContext context)
        {
            lock (this.receivedContexts)
            {
                this.receivedContexts.AddLast(context);
            }
        }

        public bool ShouldCommit(IConsumerContext context, out IConsumerContext lastProcessedContext)
        {
            lastProcessedContext = null;

            lock (this.receivedContexts)
            {
                if (!this.receivedContexts.Any())
                {
                    throw new InvalidOperationException(
                        $"There is no offsets in the received queue. Call {nameof(this.Enqueue)} first");
                }

                if (context.Offset != this.receivedContexts.First.Value.Offset)
                {
                    this.processedContexts.Add(context.Offset, context);
                    return false;
                }

                do
                {
                    lastProcessedContext = this.receivedContexts.First.Value;
                    this.receivedContexts.RemoveFirst();
                } while (this.receivedContexts.Count > 0 && this.processedContexts.Remove(this.receivedContexts.First.Value.Offset));
            }

            return true;
        }

        public Task WaitContextsCompletionAsync() => Task.WhenAll(this.receivedContexts.Select(x => x.Completion));
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KafkaFlow.Consumers;

internal class PartitionOffsets
{
    private readonly SortedDictionary<long, IConsumerContext> _processedContexts = new ();
    private readonly LinkedList<IConsumerContext> _receivedContexts = new ();

    public IConsumerContext DequeuedContext { get; private set; }

    public void Enqueue(IConsumerContext context)
    {
        lock (_receivedContexts)
        {
            _receivedContexts.AddLast(context);
        }
    }

    public bool TryDequeue(IConsumerContext context)
    {
        this.DequeuedContext = null;

        lock (_receivedContexts)
        {
            if (!_receivedContexts.Any())
            {
                throw new InvalidOperationException(
                    $"There is no offsets in the received queue. Call {nameof(this.Enqueue)} first");
            }

            if (context.Offset != _receivedContexts.First.Value.Offset)
            {
                _processedContexts.Add(context.Offset, context);
                return false;
            }

            do
            {
                this.DequeuedContext = _receivedContexts.First.Value;
                _receivedContexts.RemoveFirst();
            } while (_receivedContexts.Count > 0 && _processedContexts.Remove(_receivedContexts.First.Value.Offset));
        }

        return true;
    }

    public Task WaitContextsCompletionAsync()
    {
        List<Task> tasks;

        lock (_receivedContexts)
        {
            tasks = _receivedContexts
                .Select(x => (Task)x.Completion)
                .ToList();
        }

        return Task.WhenAll(tasks);
    }
}

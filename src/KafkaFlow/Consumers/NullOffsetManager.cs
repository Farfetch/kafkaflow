using System.Threading.Tasks;

namespace KafkaFlow.Consumers;

internal class NullOffsetManager : IOffsetManager
{
    public void Enqueue(IConsumerContext context)
    {
        // Do nothing
    }

    public void MarkAsProcessed(IConsumerContext offset)
    {
        // Do nothing
    }

    public Task WaitContextsCompletionAsync() => Task.CompletedTask;
}

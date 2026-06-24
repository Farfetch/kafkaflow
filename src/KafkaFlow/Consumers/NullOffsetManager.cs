using System.Threading.Tasks;

namespace KafkaFlow.Consumers;

internal class NullOffsetManager : IOffsetManager
{
    public bool Enqueue(IConsumerContext context) => true;

    public void MarkAsProcessed(IConsumerContext offset)
    {
        // Do nothing
    }

    public Task WaitContextsCompletionAsync() => Task.CompletedTask;
}

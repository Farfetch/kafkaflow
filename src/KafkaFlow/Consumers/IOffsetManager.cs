using System.Threading.Tasks;

namespace KafkaFlow.Consumers;

internal interface IOffsetManager
{
    void Enqueue(IConsumerContext context);

    void MarkAsProcessed(IConsumerContext offset);

    Task WaitContextsCompletionAsync();
}

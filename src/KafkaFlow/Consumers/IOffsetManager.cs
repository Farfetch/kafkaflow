using System.Threading.Tasks;

namespace KafkaFlow.Consumers;

internal interface IOffsetManager
{
    // Returns false when the context's partition is not tracked (e.g. was revoked).
    // The caller must skip further processing if false is returned.
    bool Enqueue(IConsumerContext context);

    void MarkAsProcessed(IConsumerContext offset);

    Task WaitContextsCompletionAsync();
}

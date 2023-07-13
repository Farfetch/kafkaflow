namespace KafkaFlow.Consumers
{
    using System.Threading.Tasks;

    internal interface IOffsetManager
    {
        void Enqueue(IConsumerContext context);

        void MarkAsProcessed(IConsumerContext offset);

        Task WaitContextsCompletionAsync();
    }
}

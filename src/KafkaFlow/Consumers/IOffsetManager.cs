namespace KafkaFlow.Consumers
{
    using System.Threading.Tasks;

    internal interface IOffsetManager
    {
        void MarkAsProcessed(IConsumerContext offset);

        Task WaitOffsetsCompletionAsync();
    }
}

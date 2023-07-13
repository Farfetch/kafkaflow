namespace KafkaFlow.Consumers
{
    using System.Threading.Tasks;

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
}

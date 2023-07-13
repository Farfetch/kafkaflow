namespace KafkaFlow.Consumers
{
    using System.Threading.Tasks;
    using KafkaFlow;

    internal class NullOffsetCommitter : IOffsetCommitter
    {
        public void Dispose()
        {
            // Do nothing
        }

        public void MarkAsProcessed(TopicPartitionOffset tpo)
        {
            // Do nothing
        }

        public Task StartAsync() => Task.CompletedTask;

        public Task StopAsync() => Task.CompletedTask;
    }
}

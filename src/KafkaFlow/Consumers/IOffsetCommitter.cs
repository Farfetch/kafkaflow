namespace KafkaFlow.Consumers
{
    using System.Threading.Tasks;

    internal interface IOffsetCommitter
    {
        void MarkAsProcessed(TopicPartitionOffset tpo);

        Task StartAsync();

        Task StopAsync();
    }
}

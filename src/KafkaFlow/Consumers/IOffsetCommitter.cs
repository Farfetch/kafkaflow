namespace KafkaFlow.Consumers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using KafkaFlow.Configuration;

    internal interface IOffsetCommitter
    {
        List<PendingOffsetsStatisticsHandler> PendingOffsetsStatisticsHandlers { get; }

        void MarkAsProcessed(TopicPartitionOffset tpo);

        Task StartAsync();

        Task StopAsync();
    }
}

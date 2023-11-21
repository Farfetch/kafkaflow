using System.Collections.Generic;
using System.Threading.Tasks;
using KafkaFlow.Configuration;

namespace KafkaFlow.Consumers
{
    internal interface IOffsetCommitter
    {
        List<PendingOffsetsStatisticsHandler> PendingOffsetsStatisticsHandlers { get; }

        void MarkAsProcessed(TopicPartitionOffset tpo);

        Task StartAsync();

        Task StopAsync();
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using KafkaFlow.Configuration;

namespace KafkaFlow.Consumers;

internal class NullOffsetCommitter : IOffsetCommitter
{
    public List<PendingOffsetsStatisticsHandler> PendingOffsetsStatisticsHandlers { get; } = new();

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

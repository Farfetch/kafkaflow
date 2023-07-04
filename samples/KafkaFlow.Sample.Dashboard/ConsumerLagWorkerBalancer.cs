namespace KafkaFlow.Sample.Dashboard;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Confluent.Kafka;
using KafkaFlow.Clusters;
using KafkaFlow.Configuration;
using KafkaFlow.Consumers;
using TopicMetadata = KafkaFlow.TopicMetadata;
using TopicPartitionOffset = KafkaFlow.TopicPartitionOffset;

public class ConsumerLagWorkerBalancer
{
    private readonly IClusterManager clusterManager;
    private readonly IConsumerAccessor consumerAccessor;
    private readonly int totalConsumerWorkers;
    private readonly int maxInstanceWorkers;

    public ConsumerLagWorkerBalancer(
        IClusterManager clusterManager,
        IConsumerAccessor consumerAccessor,
        int totalConsumerWorkers,
        int maxInstanceWorkers)
    {
        this.clusterManager = clusterManager;
        this.consumerAccessor = consumerAccessor;
        this.totalConsumerWorkers = totalConsumerWorkers;
        this.maxInstanceWorkers = maxInstanceWorkers;
    }

    public async Task<int> GetWorkersCountAsync(WorkersCountContext context)
    {
        var topicsMetadata = await this.GetTopicsMetadataAsync(context);

        var lastOffsets = this.GetPartitionsLastOffset(context.ConsumerName, topicsMetadata);

        var partitionsOffset = await this.clusterManager.GetConsumerGroupOffsetsAsync(
            context.ConsumerGroupId,
            context.AssignedTopicsPartitions.Select(t => t.Name));

        var partitionsLag = CalculatePartitionsLag(lastOffsets, partitionsOffset);
        var myLag = CalculateMyPartitionsLag(context, partitionsLag);

        decimal totalConsumerLag = Math.Max(partitionsLag.Sum(p => p.Lag), 1);

        var ratio = myLag / totalConsumerLag;

        var workers = (int)Math.Round(this.totalConsumerWorkers * ratio);

        if (workers > this.maxInstanceWorkers)
        {
            return this.maxInstanceWorkers;
        }

        return workers < 1 ? 1 : workers;
    }

    private static long CalculateMyPartitionsLag(
        WorkersCountContext context,
        IReadOnlyList<(string Topic, int Partition, long Lag)> partitionsLag)
    {
        return partitionsLag
            .Where(
                partitionLag => context.AssignedTopicsPartitions
                    .Any(
                        topic => topic.Name == partitionLag.Topic &&
                                 topic.Partitions.Any(p => p == partitionLag.Partition)))
            .Sum(partitionLag => partitionLag.Lag);
    }

    private static IReadOnlyList<(string Topic, int Partition, long Lag)> CalculatePartitionsLag(
        IEnumerable<(string Topic, int Partition, long Offset)> lastOffsets,
        IEnumerable<TopicPartitionOffset> currentPartitionsOffset)
    {
        return lastOffsets
            .Select(
                last =>
                {
                    var currentOffset = currentPartitionsOffset
                        .Where(current => current.Topic == last.Topic && current.Partition == last.Partition)
                        .Select(current => current.Offset)
                        .FirstOrDefault(0);

                    var lastOffset = Math.Max(0, last.Offset);
                    currentOffset = Math.Max(0, currentOffset);

                    return (last.Topic, last.Partition, lastOffset - currentOffset);
                })
            .ToList();
    }

    private IReadOnlyList<(string TopicName, int Partition, long Offset)> GetPartitionsLastOffset(
        string consumerName,
        IEnumerable<(string Name, TopicMetadata Metadata)> topicsMetadata)
    {
        var consumer = this.consumerAccessor[consumerName];

        var offsets = new List<(string TopicName, int Partition, long Offset)>();

        foreach (var topic in topicsMetadata)
        {
            foreach (var partition in topic.Metadata.Partitions)
            {
                offsets.Add(
                    (
                        topic.Name,
                        partition.Id,
                        consumer.QueryWatermarkOffsets(
                                new TopicPartition(topic.Name, new Partition(partition.Id)),
                                TimeSpan.FromSeconds(30))
                            .High.Value));
            }
        }

        return offsets;
    }

    private async Task<IReadOnlyList<(string Name, TopicMetadata Metadata)>> GetTopicsMetadataAsync(WorkersCountContext context)
    {
        var topicsMetadata = new List<(string Name, TopicMetadata Metadata)>(context.AssignedTopicsPartitions.Count);

        foreach (var topic in context.AssignedTopicsPartitions)
        {
            topicsMetadata.Add((topic.Name, await this.clusterManager.GetTopicMetadataAsync(topic.Name)));
        }

        return topicsMetadata;
    }
}

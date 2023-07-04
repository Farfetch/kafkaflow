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

public class ConsumerLagWorkersCountCalculator
{
    private const int BaseWorkersCount = 5;
    private const int MaxWorkersCount = 20;

    private readonly IClusterManager clusterManager;
    private readonly IConsumerAccessor consumerAccessor;

    public ConsumerLagWorkersCountCalculator(
        IClusterManager clusterManager,
        IConsumerAccessor consumerAccessor)
    {
        this.clusterManager = clusterManager;
        this.consumerAccessor = consumerAccessor;
    }

    public async Task<int> CalculateAsync(WorkersCountContext context)
    {
        var topicsMetadata = await this.GetTopicsMetadataAsync(context);

        decimal totalPartitions = topicsMetadata.Sum(x => x.Metadata.Partitions.Count);

        // if only one instance exists, use just one worker to improve batch processing and use less resources
        decimal assignedPartitionCount = context.AssignedTopicsPartitions.Sum(p => p.Partitions.Count());

        // if (assignedPartitionCount == totalPartitions)
        // {
        //     return 1;
        // }

        var lastOffsets = this.GetPartitionsLastOffset(context.ConsumerName, topicsMetadata);

        var partitionsOffset = await this.clusterManager.GetConsumerGroupOffsetsAsync(
            context.ConsumerGroupId,
            context.AssignedTopicsPartitions.Select(t => t.Name));

        var partitionsLag = CalculatePartitionsLag(lastOffsets, partitionsOffset);
        var myLag = CalculateMyPartitionsLag(context, partitionsLag);

        decimal totalConsumerLag = partitionsLag.Sum(p => p.Lag);

        var ratio = myLag / totalConsumerLag;

        var workers = (int)Math.Round(MaxWorkersCount * ratio);
        return workers switch
        {
            > MaxWorkersCount => MaxWorkersCount,
            < 1 => 1,
            _ => workers
        };
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

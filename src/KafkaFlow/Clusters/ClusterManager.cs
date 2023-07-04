namespace KafkaFlow.Clusters
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Confluent.Kafka;
    using Confluent.Kafka.Admin;
    using KafkaFlow.Configuration;
    using TopicMetadata = KafkaFlow.TopicMetadata;
    using TopicPartitionOffset = KafkaFlow.TopicPartitionOffset;

    internal class ClusterManager : IClusterManager, IDisposable
    {
        private readonly ILogHandler logHandler;
        private readonly Lazy<IAdminClient> lazyAdminClient;
        private readonly ClusterConfiguration configuration;

        private readonly ConcurrentDictionary<string, TopicMetadata> topicMetadataCache = new();

        public ClusterManager(ILogHandler logHandler, ClusterConfiguration configuration)
        {
            this.logHandler = logHandler;
            this.configuration = configuration;

            this.lazyAdminClient = new Lazy<IAdminClient>(
                () =>
                    new AdminClientBuilder(
                            new AdminClientConfig
                            {
                                BootstrapServers = string.Join(",", configuration.Brokers),
                            })
                        .Build());
        }

        public string ClusterName => this.configuration.Name;

        public ValueTask<TopicMetadata> GetTopicMetadataAsync(string topicName)
        {
            return new ValueTask<TopicMetadata>(
                this.topicMetadataCache.GetOrAdd(
                    topicName,
                    _ =>
                    {
                        var metadata = this.lazyAdminClient.Value.GetMetadata(topicName, TimeSpan.FromSeconds(30));

                        if (!metadata.Topics.Any())
                        {
                            return new TopicMetadata(string.Empty, Array.Empty<TopicPartitionMetadata>());
                        }

                        return new TopicMetadata(
                            metadata.Topics[0].Topic,
                            metadata.Topics[0]
                                .Partitions
                                .Select(p => new TopicPartitionMetadata(p.PartitionId))
                                .ToList());
                    }));
        }

        public async Task<IEnumerable<TopicPartitionOffset>> GetConsumerGroupOffsetsAsync(
            string consumerGroup,
            IEnumerable<string> topicsName)
        {
            var topicsMetadata = new List<(string Name, TopicMetadata Metadata)>();

            foreach (var name in topicsName)
            {
                topicsMetadata.Add((name, await this.GetTopicMetadataAsync(name)));
            }

            var topics =
                topicsMetadata
                    .SelectMany(
                        topic => topic.Metadata.Partitions.Select(
                            partition => new TopicPartition(
                                topic.Name,
                                new Partition(partition.Id))))
                    .ToList();

            var result = await this.lazyAdminClient.Value.ListConsumerGroupOffsetsAsync(
                new[] { new ConsumerGroupTopicPartitions(consumerGroup, topics) });

            if (!result.Any())
            {
                return Enumerable.Empty<TopicPartitionOffset>();
            }

            return result[0]
                .Partitions
                .Select(p => new TopicPartitionOffset(p.Topic, p.Partition.Value, p.Offset.Value))
                .ToList();
        }

        public async Task CreateIfNotExistsAsync(IEnumerable<TopicConfiguration> configurations)
        {
            try
            {
                var topics = configurations
                    .Select(
                        topicConfiguration => new TopicSpecification
                        {
                            Name = topicConfiguration.Name,
                            ReplicationFactor = topicConfiguration.Replicas,
                            NumPartitions = topicConfiguration.Partitions,
                        })
                    .ToArray();

                await this.lazyAdminClient.Value.CreateTopicsAsync(topics);
            }
            catch (CreateTopicsException exception)
            {
                var hasNonExpectedErrors = false;
                foreach (var exceptionResult in exception.Results.Where(report => report.Error.IsError))
                {
                    if (exceptionResult.Error.Code == ErrorCode.TopicAlreadyExists)
                    {
                        this.logHandler.Warning(
                            "An error occurred creating topic {Topic}: {Reason}",
                            new
                            {
                                exceptionResult.Topic,
                                exceptionResult.Error.Reason,
                            });
                        continue;
                    }

                    hasNonExpectedErrors = true;
                }

                if (hasNonExpectedErrors)
                {
                    this.logHandler.Error(
                        "An error occurred creating topics",
                        exception,
                        new
                        {
                            Servers = this.configuration.Brokers,
                        });
                    throw;
                }
            }
        }

        public void Dispose()
        {
            if (this.lazyAdminClient.IsValueCreated)
            {
                this.lazyAdminClient.Value.Dispose();
            }
        }
    }
}

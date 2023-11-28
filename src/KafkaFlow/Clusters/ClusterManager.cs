using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using KafkaFlow.Configuration;

namespace KafkaFlow.Clusters
{
    internal class ClusterManager : IClusterManager, IDisposable
    {
        private readonly ILogHandler _logHandler;
        private readonly Lazy<IAdminClient> _lazyAdminClient;
        private readonly ClusterConfiguration _configuration;

        private readonly ConcurrentDictionary<string, TopicMetadata> _topicMetadataCache = new();

        public ClusterManager(ILogHandler logHandler, ClusterConfiguration configuration)
        {
            _logHandler = logHandler;
            _configuration = configuration;

            _lazyAdminClient = new Lazy<IAdminClient>(
                () =>
                {
                    var config = new AdminClientConfig
                    {
                        BootstrapServers = string.Join(",", configuration.Brokers),
                    };

                    config.ReadSecurityInformationFrom(configuration);

                    return new AdminClientBuilder(config)
                        .Build();
                });
        }

        public string ClusterName => _configuration.Name;

        public ValueTask<TopicMetadata> GetTopicMetadataAsync(string topicName)
        {
            return new ValueTask<TopicMetadata>(
                _topicMetadataCache.GetOrAdd(
                    topicName,
                    _ =>
                    {
                        var metadata = _lazyAdminClient.Value.GetMetadata(topicName, TimeSpan.FromSeconds(30));

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

            var result = await _lazyAdminClient.Value.ListConsumerGroupOffsetsAsync(
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

                await _lazyAdminClient.Value.CreateTopicsAsync(topics);
            }
            catch (CreateTopicsException exception)
            {
                var hasNonExpectedErrors = false;
                foreach (var exceptionResult in exception.Results.Where(report => report.Error.IsError))
                {
                    if (exceptionResult.Error.Code == ErrorCode.TopicAlreadyExists)
                    {
                        _logHandler.Warning(
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
                    _logHandler.Error(
                        "An error occurred creating topics",
                        exception,
                        new
                        {
                            Servers = _configuration.Brokers,
                        });
                    throw;
                }
            }
        }

        public void Dispose()
        {
            if (_lazyAdminClient.IsValueCreated)
            {
                _lazyAdminClient.Value.Dispose();
            }
        }
    }
}

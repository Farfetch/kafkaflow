namespace KafkaFlow.Client.Metrics
{
    using System.Linq;
    using System.Threading.Tasks;
    using KafkaFlow.Client.Core;
    using KafkaFlow.Client.Exceptions;
    using KafkaFlow.Client.Protocol.Messages;

    class MetricReader : IMetricReader
    {
        private readonly IKafkaCluster Cluster;

        private readonly ConcurrentAsyncDictionary<string, IMetadataResponse.ITopic> metadataCache =
            new();

        public MetricReader(IKafkaCluster cluster)
        {
            this.Cluster = cluster;
        }

        public async Task<long> GetLagAsync(string topic, string consumerGroup)
        {
            await this.Cluster.EnsureInitializationAsync();

            var topicMetadata = await this.GetTopicMetadataAsync(topic);
            var partitions = topicMetadata.Partitions.Select(t => t.Id).ToArray();

            var consumerGroupOffsets = (await this.GetConsumerGroupOffsetsAsync(
                    topic,
                    partitions,
                    consumerGroup))
                .Partitions
                .Where(p => p.ErrorCode == (short)ErrorCode.None)
                .ToDictionary(p => p.Id, p => p.CommittedOffset);

            var topicOffsets = await this.GetTopicOffsetsAsync(topic, partitions);

            return topicOffsets.Partitions
                .Where(p => p.ErrorCode == (short)ErrorCode.None)
                .Sum(p => p.Offset -
                          (consumerGroupOffsets.ContainsKey(p.PartitionIndex) ?
                              consumerGroupOffsets[p.PartitionIndex] :
                              0));
        }

        private ValueTask<IMetadataResponse.ITopic> GetTopicMetadataAsync(string topicName)
        {
            return this.metadataCache.GetOrAddAsync(
                topicName,
                async () =>
                {
                    var host = this.Cluster.AnyBroker;

                    var requestFactory = await host.GetRequestFactoryAsync();

                    var request = requestFactory.CreateMetadata();

                    var topic = request.CreateTopic();
                    topic.Name = topicName;
                    request.Topics = new[] { topic };

                    var metadata = await host.Connection.SendAsync(request).ConfigureAwait(false);

                    if (!metadata.Topics.Any())
                    {
                        throw new TopicNotExistsException(topicName);
                    }

                    return metadata.Topics[0];
                });
        }

        private async Task<IOffsetFetchResponse.ITopic> GetConsumerGroupOffsetsAsync(string topicName, int[] partitions,
            string groupName)
        {
            var host = this.Cluster.AnyBroker;

            var requestFactory = await host.GetRequestFactoryAsync();

            var request = requestFactory.CreateOffsetFetch(groupName, topicName, partitions);

            var committedOffsets = await host.Connection.SendAsync(request).ConfigureAwait(false);

            if (committedOffsets.Error != ErrorCode.None)
            {
                throw new MetricReaderException(committedOffsets.Error, topicName, partitions, groupName);
            }

            return committedOffsets.Topics[0];
        }

        private async Task<IListOffsetsResponse.ITopic> GetTopicOffsetsAsync(string topicName, int[] partitions)
        {
            var host = this.Cluster.AnyBroker;

            var requestFactory = await host.GetRequestFactoryAsync();

            var request = requestFactory.CreateListOffset(topicName, partitions);

            var topicOffsets = await host.Connection.SendAsync(request).ConfigureAwait(false);

            return topicOffsets.Topics[0];
        }
    }
}

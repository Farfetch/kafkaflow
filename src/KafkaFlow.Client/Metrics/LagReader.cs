namespace KafkaFlow.Client.Metrics
{
    using System.Linq;
    using System.Threading.Tasks;
    using KafkaFlow.Client.Core;
    using KafkaFlow.Client.Exceptions;
    using KafkaFlow.Client.Protocol.Messages;
    using Protocol.Messages.Implementations.OffsetFetch;

    internal class LagReader : ILagReader
    {
        private readonly IKafkaCluster cluster;

        private IKafkaBroker Host => this.cluster.AnyBroker;

        private readonly ConcurrentAsyncDictionary<string, IMetadataResponse.ITopic> metadataCache = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="LagReader"/> class.
        /// </summary>
        /// <param name="cluster"></param>
        public LagReader(IKafkaCluster cluster)
        {
            this.cluster = cluster;
        }

        public async Task<long> GetLagAsync(string topic, string consumerGroup)
        {
            await this.cluster.EnsureInitializationAsync().ConfigureAwait(false);
            var topicMetadata = await this.GetTopicMetadataAsync(topic).ConfigureAwait(false);

            var partitions = topicMetadata.Partitions.Select(t => t.Id).ToArray();

            var consumerGroupOffsets = await this.GetConsumerGroupOffsetsAsync(
                    topic,
                    partitions,
                    consumerGroup)
                .ConfigureAwait(false);
            return 0;

            var validOffsets = consumerGroupOffsets
                .Partitions
                .Where(p => p.ErrorCode == (short)ErrorCode.None && p.CommittedOffset > -1)
                .ToDictionary(p => p.Id, p => p.CommittedOffset);
            return 0;
            var topicOffsets = await this.GetTopicOffsetsAsync(topic, partitions).ConfigureAwait(false);

            return topicOffsets
                .Partitions
                .Where(p => p.ErrorCode == (short)ErrorCode.None)
                .Sum(p => p.Offset -
                          (validOffsets.ContainsKey(p.PartitionIndex)
                              ? validOffsets[p.PartitionIndex]
                              : 0));
        }

        private ValueTask<IMetadataResponse.ITopic> GetTopicMetadataAsync(string topicName)
        {
            return this.metadataCache.GetOrAddAsync(
                topicName,
                async () =>
                {
                    var requestFactory = await this.Host.GetRequestFactoryAsync();

                    var request = requestFactory.CreateMetadata();
                    request.Topics = new[] { request.CreateTopic(topicName) };

                    var metadata = await this.Host.Connection.SendAsync(request).ConfigureAwait(false);

                    if (!metadata.Topics.Any())
                    {
                        throw new TopicNotExistsException(topicName);
                    }

                    return metadata.Topics.First();
                });
        }

        private async Task<IOffsetFetchResponse.ITopic> GetConsumerGroupOffsetsAsync(
            string topicName,
            int[] partitions,
            string groupName)
        {
            var requestFactory = await this.Host.GetRequestFactoryAsync().ConfigureAwait(false);

            var committedOffsets = await this.Host
                .Connection
                .SendAsync(requestFactory.CreateOffsetFetch(groupName, topicName, partitions))
                .ConfigureAwait(false);

            if (committedOffsets.Error != ErrorCode.None)
            {
                throw new LagReaderException(committedOffsets.Error, topicName, partitions, groupName);
            }

            return committedOffsets.Topics.First();
        }

        private async Task<IListOffsetsResponse.ITopic> GetTopicOffsetsAsync(string topicName, int[] partitions)
        {
            var requestFactory = await this.Host.GetRequestFactoryAsync().ConfigureAwait(false);

            var topicOffsets = await this.Host
                .Connection
                .SendAsync(requestFactory.CreateListOffset(topicName, partitions))
                .ConfigureAwait(false);

            return topicOffsets.Topics.First();
        }
    }
}

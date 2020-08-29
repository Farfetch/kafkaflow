namespace KafkaFlow.Client.Producers
{
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading.Tasks;
    using KafkaFlow.Client.Core;
    using KafkaFlow.Client.Exceptions;
    using KafkaFlow.Client.Extensions;
    using KafkaFlow.Client.Producers.Partitioners;
    using KafkaFlow.Client.Protocol.Messages;

    internal class Producer : IProducer
    {
        private readonly IKafkaCluster cluster;
        private readonly ProducerConfiguration configuration;
        private readonly IProducerPartitioner partitioner;

        private readonly ConcurrentAsyncDictionary<string, IMetadataResponse.ITopic> metadataCache =
            new ConcurrentAsyncDictionary<string, IMetadataResponse.ITopic>();

        private readonly ConcurrentDictionary<int, ProducerSender> senders =
            new ConcurrentDictionary<int, ProducerSender>();

        public Producer(
            IKafkaCluster cluster,
            ProducerConfiguration configuration,
            IProducerPartitioner partitioner)
        {
            this.cluster = cluster;
            this.configuration = configuration;
            this.partitioner = partitioner;
        }

        public async Task<ProduceResult> ProduceAsync(ProduceData data)
        {
            await this.cluster.EnsureInitializationAsync().ConfigureAwait(false);

            var topic = await this.GetTopicMetadataAsync(data.Topic).ConfigureAwait(false);

            var partitionId = this.partitioner.GetPartition(topic.Partitions.Length, data.Key);

            //TODO: update cache and retry when NotLeaderForPartition occur

            var sender = this.GetHostSender(topic.Partitions[partitionId].LeaderId);

            var completionSource = new TaskCompletionSource<ProduceResult>();

            await sender
                .EnqueueAsync(new ProduceQueueItem(data, partitionId, completionSource))
                .ConfigureAwait(false);

            return await completionSource.Task.ConfigureAwait(false);
        }

        private ProducerSender GetHostSender(int partitionLeaderId)
        {
            return this.senders
                .SafeGetOrAdd(
                    partitionLeaderId,
                    id => new ProducerSender(
                        this.cluster.GetHost(id),
                        this.configuration));
        }

        private ValueTask<IMetadataResponse.ITopic> GetTopicMetadataAsync(string topicName)
        {
            return this.metadataCache.GetOrAddAsync(
                topicName,
                async () =>
                {
                    var host = this.cluster.AnyHost;

                    var request = host.RequestFactory.CreateMetadata();

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
    }
}

namespace KafkaFlow.Client.Producers
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading.Tasks;
    using KafkaFlow.Client.Core;
    using KafkaFlow.Client.Exceptions;
    using KafkaFlow.Client.Extensions;
    using KafkaFlow.Client.Producers.Partitioners;
    using KafkaFlow.Client.Protocol.Messages;

    public class Producer : IProducer
    {
        private readonly IKafkaCluster cluster;
        private readonly ProducerConfiguration configuration;
        private readonly IProducerPartitioner partitioner;

        private readonly ConcurrentAsyncDictionary<string, IMetadataResponse.ITopic> metadataCache =
            new();

        private readonly ConcurrentDictionary<int, ProducerSender> senders =
            new();

        public Producer(
            IKafkaCluster cluster,
            ProducerConfiguration configuration,
            IProducerPartitioner partitioner)
        {
            this.cluster = cluster;
            this.configuration = configuration;
            this.partitioner = partitioner;
        }

        public async Task<ProduceItem> ProduceAsync(
            string topicName,
            Memory<byte> key,
            Memory<byte> value,
            Headers headers)
        {
            await this.cluster.EnsureInitializationAsync().ConfigureAwait(false);

            var topic = await this.GetTopicMetadataAsync(topicName).ConfigureAwait(false);

            var partitionId = this.partitioner.GetPartition(topic.Partitions.Length, key);

            //TODO: update cache and retry when NotLeaderForPartition occur

            var sender = this.GetBrokerSender(topic.Partitions.First(x => x.Id == partitionId).LeaderId);

            var produceItem = new ProduceItem(
                topicName,
                key,
                value,
                headers,
                partitionId);

            await sender
                .EnqueueAsync(produceItem)
                .ConfigureAwait(false);

            return await produceItem.CompletionTask.ConfigureAwait(false);
        }

        private ProducerSender GetBrokerSender(int partitionLeaderId)
        {
            return this.senders
                .ThreadSafeGetOrAdd(
                    partitionLeaderId,
                    id => new ProducerSender(
                        this.cluster.GetBroker(id),
                        this.configuration));
        }

        private ValueTask<IMetadataResponse.ITopic> GetTopicMetadataAsync(string topicName)
        {
            return this.metadataCache.GetOrAddAsync(
                topicName,
                async () =>
                {
                    var host = this.cluster.AnyBroker;

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
    }
}

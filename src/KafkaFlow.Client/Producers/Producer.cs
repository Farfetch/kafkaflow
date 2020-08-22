namespace KafkaFlow.Client.Producers
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading.Tasks;
    using KafkaFlow.Client.Core;
    using KafkaFlow.Client.Exceptions;
    using KafkaFlow.Client.Messages;
    using KafkaFlow.Client.Producers.Partitioners;

    internal class Producer : IProducer
    {
        private readonly IKafkaCluster cluster;
        private readonly ProducerConfiguration configuration;

        private readonly IProducerPartitioner partitioner;

        private readonly ConcurrentAsyncDictionary<string, MetadataResponse.Topic> metadataCache =
            new ConcurrentAsyncDictionary<string, MetadataResponse.Topic>();

        private readonly ConcurrentDictionary<int, Lazy<ProducerSender>> senders =
            new ConcurrentDictionary<int, Lazy<ProducerSender>>();

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
            try
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
            catch (Exception e)
            {
                throw new ProduceException(e);
            }
        }

        private ProducerSender GetHostSender(int partitionLeaderId)
        {
            return this.senders
                .GetOrAdd(
                    partitionLeaderId,
                    id => new Lazy<ProducerSender>(
                        () => new ProducerSender(
                            this.cluster.GetHost(id),
                            this.configuration)))
                .Value;
        }

        private ValueTask<MetadataResponse.Topic> GetTopicMetadataAsync(string topicName)
        {
            return this.metadataCache.GetOrAddAsync(
                topicName,
                async () =>
                {
                    var metadata = await this.cluster.AnyHost
                        .SendAsync(new MetadataRequest(new[] { topicName }))
                        .ConfigureAwait(false);

                    if (!metadata.Topics.Any())
                    {
                        throw new TopicNotExistsException(topicName);
                    }

                    return metadata.Topics[0];
                });
        }
    }
}

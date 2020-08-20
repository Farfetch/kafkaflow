namespace KafkaFlow.Client.Metadata
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    internal class MetadataClient : IMetadataClient
    {
        private readonly IKafkaCluster cluster;

        public MetadataClient(IKafkaCluster cluster)
        {
            this.cluster = cluster;
        }

        public async Task<ConsumerGroupLagInfo> GetConsumerGroupLagAsync(string consumerGroup, IEnumerable<string> topics)
        {
            var broker = this.cluster.Brokers.Values.First();

            var metadata = await broker.Connection.SendAsync(
                broker.RequestFactory
                    .CreateMetadata()
                    .AddTopics(topics));

            var offsetsFetchRequest = broker.RequestFactory.CreateOffsetFetch(consumerGroup);
            var listOffsetsRequest = broker.RequestFactory.CreateListOffset();

            foreach (var topic in metadata.Topics)
            {
                var partitions = topic.Partitions
                    .Select(x => x.Id)
                    .ToList();

                offsetsFetchRequest.AddTopic(topic.Name, partitions);
                listOffsetsRequest.AddTopic(topic.Name, partitions);
            }

            var consumerOffsetsTask = broker.Connection.SendAsync(offsetsFetchRequest);
            var topicOffsetsTask = broker.Connection.SendAsync(listOffsetsRequest);

            await Task.WhenAll(consumerOffsetsTask, topicOffsetsTask).ConfigureAwait(false);

            var indexedOffsets = consumerOffsetsTask.Result.Topics
                .SelectMany(t => t.Partitions.Select(p => (t.Name, p.Id, p.CommittedOffset)))
                .ToDictionary(x => (x.Name, x.Id), x => x.CommittedOffset);

            return new ConsumerGroupLagInfo(
                topicOffsetsTask.Result.Topics
                    .Select(
                        topic => new ConsumerGroupLagInfo.Topic(
                            topic.Name,
                            topic.Partitions
                                .Select(
                                    partition => new ConsumerGroupLagInfo.Partition(
                                        partition.Id,
                                        indexedOffsets.TryGetValue((topic.Name, partition.Id), out var lag) ?
                                            partition.Offset - lag :
                                            partition.Offset))
                                .ToList()))
                    .ToList());
        }

        public async Task<TopicLagInfo> GetTopicLagAsync(string topic, IReadOnlyCollection<string> consumerGroups)
        {
            var broker = this.cluster.Brokers.Values.First();

            var metadata = await broker.Connection.SendAsync(
                broker.RequestFactory
                    .CreateMetadata()
                    .AddTopic(topic));

            var partitions = metadata.Topics[0]
                .Partitions
                .Select(p => p.Id)
                .ToList();

            var topicLastOffsetsTask = broker.Connection
                .SendAsync(
                    broker.RequestFactory
                        .CreateListOffset()
                        .AddTopic(topic, partitions));

            var consumerOffsetsTasks = consumerGroups.Select(
                    consumerGroup =>
                    {
                        var offsetsFetchRequest = broker.RequestFactory.CreateOffsetFetch(consumerGroup);
                        offsetsFetchRequest.AddTopic(topic, partitions);

                        return broker.Connection.SendAsync(offsetsFetchRequest);
                    })
                .ToList();

            await topicLastOffsetsTask.ConfigureAwait(false);
            await Task.WhenAll(consumerOffsetsTasks).ConfigureAwait(false);

            return new TopicLagInfo(
                consumerGroups
                    .Select(
                        (consumerGroup, index) => new TopicLagInfo.ConsumerGroup(
                            consumerGroup,
                            topicLastOffsetsTask.Result.Topics[0]
                                .Partitions
                                .Select(
                                    topicPartition =>
                                    {
                                        var consumerPartitionOffset = consumerOffsetsTasks[index]
                                            .Result.Topics[0]
                                            .Partitions.FirstOrDefault(
                                                consumerPartition => consumerPartition.Id == topicPartition.Id)
                                            ?.CommittedOffset ?? 0;

                                        consumerPartitionOffset = consumerPartitionOffset < 0 ? 0 : consumerPartitionOffset;

                                        return new TopicLagInfo.Partition(
                                            topicPartition.Id,
                                            topicPartition.Offset - consumerPartitionOffset);
                                    })
                                .ToList()))
                    .ToList());
        }
    }
}

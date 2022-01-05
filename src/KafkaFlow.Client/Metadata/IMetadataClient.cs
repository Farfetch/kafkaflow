namespace KafkaFlow.Client.Metadata
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IMetadataClient
    {
        Task<ConsumerGroupLagInfo> GetConsumerGroupLagAsync(string consumerGroup, IEnumerable<string> topics);

        Task<TopicLagInfo> GetTopicLag(string topic, IReadOnlyCollection<string> consumerGroups);
    }
}
